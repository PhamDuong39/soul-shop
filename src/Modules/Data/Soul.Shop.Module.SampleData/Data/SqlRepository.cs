using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Soul.Shop.Module.Core.Data;

namespace Soul.Shop.Module.SampleData.Data;

public class SqlRepository(ShopDbContext dbContext, ILogger<SqlRepository> logger) : ISqlRepository
{
    private readonly DbContext _dbContext = dbContext;
    private readonly ILogger _logger = logger;

    public void RunCommand(string command)
    {
        _logger.LogDebug(command);
        _dbContext.Database.ExecuteSqlRaw(command);
    }

    public void RunCommands(IEnumerable<string> commands)
    {
        using var tran = _dbContext.Database.BeginTransaction();
        foreach (var command in commands)
            _dbContext.Database.ExecuteSqlRaw(command);
        tran.Commit();
    }

    public IEnumerable<string> ParseCommand(IEnumerable<string> lines)
    {
        var sb = new StringBuilder();
        var commands = new List<string>();
        foreach (var line in lines)
            if (string.Equals(line, "GO", StringComparison.OrdinalIgnoreCase))
            {
                if (sb.Length <= 0) continue;
                var sql = sb.ToString().Replace("{", "{{").Replace("}", "}}");
                commands.Add(sql);

                sb = new StringBuilder();
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(line)) sb.Append(line);
            }

        return commands;
    }

    public IEnumerable<string> MySqlCommand(IEnumerable<string> lines)
    {
        var commands = new List<string>();
        var sb = new StringBuilder();
        foreach (var line in lines)
            if (!string.IsNullOrWhiteSpace(line))
                sb.AppendLine(line.Replace("{", "{{").Replace("}", "}}"));

        commands.Add(sb.ToString());

        return commands;
    }

    public IEnumerable<string> PostgresCommands(IEnumerable<string> lines)
    {
        return lines.Where(line => !string.IsNullOrWhiteSpace(line)).ToList();
    }

    public string GetDbConnectionType()
    {
        var dbConnectionType = _dbContext.Database.GetDbConnection().GetType();
        return dbConnectionType.ToString();
    }
}