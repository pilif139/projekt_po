using projekt_po.Model;
using projekt_po.Services;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Menu;

public class AdminMenu
{
    private readonly UserService _userService;
    private readonly IAuthService _authService;
    private readonly ReservationService _reservationService;
    public AdminMenu(UserService userService, IAuthService authService, ReservationService reservationService)
    {
        _userService = userService;
        _authService = authService;
        _reservationService = reservationService;
    }

    public void ShowAdminMenu() //function that shows admin menu
    {
        while (true)
        {
            AnsiConsole.Clear();
            var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[red]Admin menu[/]")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Add user",
                    "Delete user",
                    "List of users",
                    "Add reservation",
                    "Delete reservation",
                    "List reservations",
                    "Logout"
                }));
            switch (option)
            {
                case "Add user":
                    AddUser();
                    break;
                case "Delete user":
                    DeleteList(_userService);
                    break;
                case "List of users":
                    List(_userService);
                    break;
                case "Add reservation":
                    AddReservation();
                    break;
                case "Delete reservation":
                    DeleteList(_reservationService);
                    break;
                case "List reservations":
                    List(_reservationService);
                    break;
                case "Logout":
                    _authService.Logout();
                    return;
            }
        }
    }

    private void AddReservation()
    {
        var clients = _userService.GetAllByRole(Role.Client);
        if (clients == null || clients.Count == 0)
        {
            Console.WriteLine("No clients found.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var client = AnsiConsole.Prompt(
            new SelectionPrompt<User>()
                .Title("Choose client")
                .PageSize(15)
                .AddChoices(clients));

        string details = Prompt.GetString("Enter reservation details: ");
        DateTime date = Prompt.GetDate("Enter reservation date: ");
        Reservation newReservation = new Reservation(date, details, client.Id);
        _reservationService.Add(newReservation);
        AnsiConsole.WriteLine("Reservation added successfully.");
        Task.Delay(2000).Wait();
    }

    private void AddUser()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[red]Creating new user[/]");
        string name = AnsiConsole.Prompt(new TextPrompt<string>("Enter username: "));
        string surname = AnsiConsole.Prompt(new TextPrompt<string>("Enter surname: "));
        string password = AnsiConsole.Prompt(new TextPrompt<string>("Enter password: "));
        var role = AnsiConsole.Prompt(
            new SelectionPrompt<Role>()
                .Title("Choose role")
                .PageSize(3)
                .AddChoices(new[]
                {
                    Role.Admin,
                    Role.Worker,
                    Role.Client
                }));
        User user = new User(name, surname, password, role);
        _userService.Add(user);
        AnsiConsole.WriteLine("User added successfully.");
        Task.Delay(2000).Wait();
    }

    private void DeleteList<T>(IModelService<T> service) where T : IModelType
    {
        string modelName = typeof(T).Name;
        AnsiConsole.Clear();
        var values = service.GetAll();
        if (values == null || values.Count == 0)
        {
            Console.WriteLine("Nothing to delete.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }
        var valuesToDelete = AnsiConsole.Prompt(
            new MultiSelectionPrompt<T>()
                .Title($"Choose {modelName}s to delete")
                .NotRequired()
                .PageSize(15)
                .MoreChoicesText("[grey](Move up and down))[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to pick a user to delete, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoices(values));

        foreach (var value in valuesToDelete)
        {
            service.Delete(value.Id);
        }
        if (valuesToDelete.Count > 0)
        {
            Console.WriteLine($"{modelName}s deleted successfully.");
        }
        else
        {
            Console.WriteLine($"No {modelName}s deleted.");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private void List<T>(IModelService<T> service) where T : IModelType
    {
        AnsiConsole.Clear();
        string modelName = typeof(T).Name;
        var models = service.GetAll();
        AnsiConsole.Markup($"[red]List of {modelName}s[/]\n");
        if (models == null)
        {
            Console.WriteLine($"No {modelName}s found.");
        }
        else
        {
            foreach (var model in models)
            {
                Console.WriteLine(model);
            }
        }

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}