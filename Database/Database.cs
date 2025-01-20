using Microsoft.EntityFrameworkCore;
using projekt_po.Model;

namespace projekt_po.Database;

public class DatabaseContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string connectionString = "Server=localhost:3306;Database=projekt_po;User=root;Password=123;";
        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    }
}