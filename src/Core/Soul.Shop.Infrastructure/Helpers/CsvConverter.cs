﻿using System.Reflection;

namespace Soul.Shop.Infrastructure.Helpers;

public static class CsvConverter
{
    public static IList<T> ReadCsvStream<T>(Stream stream, bool skipFirstLine = true, string csvDelimiter = ",")
        where T : new()
    {
        var records = new List<T>();
        var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();
            var values = line.Split(csvDelimiter.ToCharArray());
            if (skipFirstLine)
            {
                skipFirstLine = false;
            }
            else
            {
                var itemTypeInGeneric = records.GetType().GetTypeInfo().GenericTypeArguments[0];
                var item = new T();
                var properties = item.GetType().GetProperties();
                for (var i = 0; i < values.Length; i++)
                    properties[i].SetValue(item, Convert.ChangeType(values[i], properties[i].PropertyType), null);

                records.Add(item);
            }
        }

        return records;
    }

    public static string ExportCsv<T>(IList<T> data, bool includeHeader = true, string csvDelimiter = ",")
        where T : new()
    {
        var type = data.GetType();

        var itemType = type.GetGenericArguments().Length > 0 ? type.GetGenericArguments()[0] : type.GetElementType();

        var stringWriter = new StringWriter();

        if (includeHeader)
            stringWriter.WriteLine(
                string.Join<string>(
                    csvDelimiter, itemType.GetProperties().Select(x => x.Name)
                )
            );

        foreach (var obj in data)
        {
            var vals = obj.GetType().GetProperties().Select(pi => new
                {
                    Value = pi.GetValue(obj, null)
                }
            );

            var line = string.Empty;
            foreach (var val in vals)
                if (val.Value != null)
                {
                    var escapeVal = val.Value.ToString();
                    if (escapeVal.Contains(',')) escapeVal = string.Concat("\"", escapeVal, "\"");
                    if (escapeVal.Contains('\r')) escapeVal = escapeVal.Replace("\r", " ");
                    if (escapeVal.Contains('\n')) escapeVal = escapeVal.Replace("\n", " ");
                    line = string.Concat(line, escapeVal, csvDelimiter);
                }
                else
                {
                    line = string.Concat(line, string.Empty, csvDelimiter);
                }

            stringWriter.WriteLine(line.TrimEnd(csvDelimiter.ToCharArray()));
        }

        return stringWriter.ToString();
    }
}