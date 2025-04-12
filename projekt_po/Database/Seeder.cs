using Bogus;
using Microsoft.EntityFrameworkCore;
using projekt_po.Model;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Database;

public class Seeder
{
    private readonly DatabaseContext _context;

    public Seeder(DatabaseContext context)
    {
        _context = context;
    }
    /// <summary>
    /// Seeds empty database with test data with bogus library.
    /// If user want it can create admin user with login and password from environment variables or default values that are "Admin" for both.
    /// </summary>
    public void Seed()
    {
        // run any new migrations before seeding
        _context.Database.Migrate();
        if(!_context.Database.EnsureCreated()) throw new InvalidOperationException("Database migration failed");
        // check if the database is already seeded
        if (_context.Users.Any())
        {
            AnsiConsole.MarkupLine("[yellow]Database already seeded.[/]");
            return;
        }
        
        var workers = GenerateUsers(10, Role.Worker);
        var clients = GenerateUsers(10, Role.Client);
        _context.Users.AddRange(workers);
        _context.Users.AddRange(clients);
        _context.SaveChanges();
        
        var lanes = GenerateLanes(10, workers);
        _context.Lanes.AddRange(lanes);
        _context.SaveChanges();
        
        var reservations = GenerateReservations(10, clients, lanes);
        _context.Reservations.AddRange(reservations);
        _context.SaveChanges();
        
        string adminEnvLogin = Environment.GetEnvironmentVariable("ADMIN_LOGIN") ?? "Admin";
        string adminEnvPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "Admin";
        if (AnsiConsole.Confirm("[yellow]Do you want to add admin user?[/]"))
        {
            AddAdminUser(adminEnvLogin, adminEnvPassword);
        }
        
        _context.SaveChanges();
        AnsiConsole.MarkupLine("[green]Database seeded successfully.[/]");
    }

    private List<User> GenerateUsers(int count, Role role)
    {
        return new Faker<User>()
            .RuleFor(u => u.Login, f => f.Internet.UserName())
            .RuleFor(u => u.Name, f => f.Name.FirstName())
            .RuleFor(u => u.Surname, f => f.Name.LastName())
            .RuleFor(u => u.Password, f => Hash.HashPassword(f.Internet.Password()))
            .RuleFor(u => u.Role, role)
            .Generate(count);
    }

    private List<Lane> GenerateLanes(int count, List<User> users)
    {
        // lane can only be managed by workers
        var workers = users.Where(u => u.Role == Role.Worker).ToList();
        return new Faker<Lane>()
            .RuleFor(l => l.Number, (f, l) => f.IndexFaker + 1)
            .RuleFor(l => l.Status, f => f.PickRandom<LaneStatus>())
            .RuleFor(l => l.Price, f => f.Finance.Amount(10, 150))
            .RuleFor(l => l.UserId, f => f.PickRandom(workers).Id)
            .Generate(count);
    }
    
    private List<Reservation> GenerateReservations(int count, List<User> users, List<Lane> lanes)
    {
        var clients = users.Where(u => u.Role == Role.Client).ToList();
        var availableLanes = lanes.Where(l => l.Status == LaneStatus.Available).ToList();
        return new Faker<Reservation>()
            .RuleFor(r => r.Date, f => f.Date.Future())
            .RuleFor(r => r.Details, f => f.Lorem.Sentence())
            .RuleFor(r => r.UserId, f => f.PickRandom(clients).Id)
            .RuleFor(r => r.LaneId, f => f.PickRandom(availableLanes).Id)
            .Generate(count);
    }

    private void AddAdminUser(string login, string password)
    {
        var adminUser = new User(login, login, login, Hash.HashPassword(password), Role.Admin);
        _context.Users.Add(adminUser);
    }
}