using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using projekt_po.Model;
using projekt_po.Utils;

namespace projekt_po.Database;

public class DatabaseContext : DbContext
{
    // Define database tables
    public DbSet<User> Users { get; init; }
    public DbSet<Reservation> Reservations { get; init; }
    public DbSet<Lane> Lanes { get; init; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Define relationships between database tables
        modelBuilder.Entity<User>()
            .HasMany(u => u.Reservations)
            .WithOne(r => r.User)
            .HasForeignKey(r => r.UserId);
        
        modelBuilder.Entity<User>()
            .HasMany(u => u.Lanes)
            .WithOne(l => l.User)
            .HasForeignKey(l => l.UserId);
        
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.User)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.UserId);
        
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Lane)
            .WithMany(l => l.Reservations)
            .HasForeignKey(r => r.LaneId);

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
        //load .env file
        Env.Load();
        var server = Environment.GetEnvironmentVariable("DB_SERVER");
        var port = Environment.GetEnvironmentVariable("DB_PORT");
        var database = Environment.GetEnvironmentVariable("DB_NAME");
        var user = Environment.GetEnvironmentVariable("DB_USER");
        var password = Environment.GetEnvironmentVariable("DB_PASSWORD");
        // Check if all variables are set
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
            // Handle invalid connection string
            Console.WriteLine("Invalid connection string: " + e.Message);
            Environment.Exit(1);

        }
        catch (MySqlException e)
        {
            // Handle MySQL connection errors
            Console.WriteLine("Error connecting to database: " + e.Message);
            Environment.Exit(1);
        }
        catch (Exception e)
        {
            // Handle other exceptions
            Console.WriteLine("Error: " + e.Message);
            Environment.Exit(1);
        }
    }
}