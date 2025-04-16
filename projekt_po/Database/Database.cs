using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using projekt_po.Model;
using projekt_po.Utils;

namespace projekt_po.Database;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; init; }
    public DbSet<Reservation> Reservations { get; init; }
    public DbSet<Lane> Lanes { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>()
            .HasMany(u => u.Reservations)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId);
        
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId);

        modelBuilder.Entity<Lane>()
            .HasMany(l => l.Reservations)
            .WithOne(r => r.Lane)
            .HasForeignKey(r => r.LaneId);
        
        modelBuilder.Entity<Lane>()
            .HasOne<User>(l => l.User)
            .WithMany(u => u.Lanes)
            .HasForeignKey(l => l.UserId);

    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        Env.Load();
        var server = Environment.GetEnvironmentVariable("DB_SERVER");
        var port = Environment.GetEnvironmentVariable("DB_PORT");
        var database = Environment.GetEnvironmentVariable("DB_NAME");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
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
            Environment.Exit(1);

        }
        catch (MySqlException e)
        {
            Console.WriteLine("Error connecting to database: " + e.Message);
            Environment.Exit(1);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error: " + e.Message);
            Environment.Exit(1);
        }
    }
}