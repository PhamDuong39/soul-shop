namespace Soul.Shop.Infrastructure;

public static class CodeGen
{
    private static readonly object obj = new();

    public static string GenRandomNumber(int length = 6)
    {
        var code = string.Empty;
        if (length <= 0)
            return code;
        var start = Convert.ToInt32(Math.Pow(10, length - 1));
        var end = Convert.ToInt32(Math.Pow(10, length));
        lock (obj)
        {
            code = new Random().Next(start, end).ToString();
        }

        return code;
    }
}