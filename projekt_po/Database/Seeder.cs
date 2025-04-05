using Bogus;
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

    public void Seed()
    {
        var fakeReservations = new Faker<Reservation>()
            .RuleFor(r => r.Date, f => f.Date.Future())
            .RuleFor(r => r.Details, f => f.Lorem.Sentences());

        var fakeUsers = new Faker<User>()
            .RuleFor(u => u.Login, f => f.Internet.UserName())
            .RuleFor(u => u.Name, f => f.Name.FirstName())
            .RuleFor(u => u.Surname, f => f.Name.LastName())
            .RuleFor(u => u.Password, f =>
            {
                var password = f.Internet.Password();
                return Hash.HashPassword(password);
            })
            .RuleFor(u => u.Role, f => f.PickRandom<Role>())
            .RuleFor(u => u.Reservations, (f, u) =>
            {
                var reservations = fakeReservations.Generate(3).ToList();
                foreach (var reservation in reservations)
                {
                    reservation.UserId = u.Id;
                    reservation.User = u;
                }
                return reservations;
            });

        var users = fakeUsers.Generate(10);
        _context.Users.AddRange(users);

        //exmaple admin user in dev environment
        // prompt
        bool createAdminUser = AnsiConsole.Prompt(
            new ConfirmationPrompt("Do you want to create example admin user?"));
        if (createAdminUser)
        {
            var adminUser = new User("Admin","Admin", "Admin", Hash.HashPassword("123"), Role.Admin);
            _context.Users.Add(adminUser);
            AnsiConsole.MarkupLine("[red]Added Admin user with login Admin and password 123[/]");    
        }
        

        _context.SaveChanges();
        foreach (var user in users)
        {
            Console.WriteLine(user);
        }
    }
}