using Microsoft.EntityFrameworkCore;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Database;

public class Cleaner
{
    private readonly DatabaseContext _context;
    private readonly ILogger _logger;
    
    public Cleaner(DatabaseContext context, ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public void CleanDatabase()
    {
        //get tables names
        var tableNames = _context.Model.GetEntityTypes().Select(t => t.GetTableName()).Distinct().ToList();
        
        // truncate all tables
        foreach (var tableName in tableNames)
        {
            _context.Database.ExecuteSqlRaw($"SET FOREIGN_KEY_CHECKS = 0; TRUNCATE TABLE `{tableName}`");
            AnsiConsole.MarkupLine($"[red]Truncated table {tableName} [/]");
        }
        _context.Database.ExecuteSql($"SET FOREIGN_KEY_CHECKS = 1");
        _context.SaveChanges();
        AnsiConsole.MarkupLine($"[green]Truncated all tables [/]");
        _logger.Log("Cleaned database");
    }

    public void CleanLogger()
    {
        _logger.Clean();
    }
}