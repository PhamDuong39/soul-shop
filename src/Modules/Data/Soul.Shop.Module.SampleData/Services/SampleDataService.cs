using Microsoft.Extensions.Options;
using Shop.Module.Core.Cache;
using Shop.Module.SampleData.ViewModels;
using Soul.Shop.Infrastructure;
using Soul.Shop.Infrastructure.Models;
using Soul.Shop.Module.Core.Abstractions.Services;
using Soul.Shop.Module.SampleData.Data;

namespace Soul.Shop.Module.SampleData.Services;

public class SampleDataService(
    ISqlRepository sqlRepository,
    IMediaService mediaService,
    IStaticCacheManager cache,
    IOptionsSnapshot<ShopOptions> options)
    : ISampleDataService
{
    private readonly IOptionsSnapshot<ShopOptions> _options = options;

    public async Task ResetToSampleData(SampleDataOption model)
    {
        if (options.Value.ShopEnv == ShopEnv.PRO)
            throw new Exception("This operation is not allowed in the official environment");

        var usePostgres = sqlRepository.GetDbConnectionType() == "Npgsql.NpgsqlConnection";
        var useSqLite = sqlRepository.GetDbConnectionType() == "Microsoft.Data.Sqlite.SqliteConnection";
        var useMySql = sqlRepository.GetDbConnectionType().Contains("MySql");

        var sampleContentFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SampleContent", model.Industry);
        var filePath = usePostgres ? Path.Combine(sampleContentFolder, "ResetToSampleData_Postgres.sql") :
            useSqLite ? Path.Combine(sampleContentFolder, "ResetToSampleData_SQLite.sql") :
            useMySql ? Path.Combine(sampleContentFolder, "ResetToSampleData_MySql.sql") :
            Path.Combine(sampleContentFolder, "ResetToSampleData.sql");

        var lines = File.ReadLines(filePath);
        var commands = usePostgres || useSqLite ? sqlRepository.PostgresCommands(lines) :
            useMySql ? sqlRepository.MySqlCommand(lines) :
            sqlRepository.ParseCommand(lines);
        sqlRepository.RunCommands(commands);

        await CopyImages(sampleContentFolder);

        cache.Clear();
    }

    private async Task CopyImages(string sampleContentFolder)
    {
        var imageFolder = Path.Combine(sampleContentFolder, "Images");
        IEnumerable<string> files = Directory.GetFiles(imageFolder);
        foreach (var file in files)
        {
            await using var stream = File.OpenRead(file);
            await mediaService.SaveMediaAsync(stream, Path.GetFileName(file));
        }
    }
}