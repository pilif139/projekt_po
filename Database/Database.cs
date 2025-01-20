using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using projekt_po.Model;
using projekt_po.Utils;

namespace projekt_po.Database;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string? server = Environment.GetEnvironmentVariable("DB_SERVER");
        string? port = Environment.GetEnvironmentVariable("DB_PORT");
        string? database = Environment.GetEnvironmentVariable("DB_NAME");
        string? user = Environment.GetEnvironmentVariable("DB_USER");
        string? password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        if (StringUtils.AreStringsNullOrEmpty(server, port, database, user, password))
        {
            throw new Exception("You need to define database variables in .env file");
        }

        string connectionString = $"Server={server};Port={port};Database={database};User={user};Password={password};";

        try
        {
            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }
        catch (ArgumentException e)
        {
            Console.WriteLine("Invalid connection string: " + e.Message);
        }
        catch (MySqlException e)
        {
            Console.WriteLine("Error connecting to database: " + e.Message);
        }

    }
}