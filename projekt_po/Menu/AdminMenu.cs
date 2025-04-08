using projekt_po.Model;
using projekt_po.Services;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Menu;

public class AdminMenu : BaseMenu
{
    private readonly UserService _userService;
    private readonly LaneService _laneService;
    private readonly IAuthService _authService;
    private readonly ReservationService _reservationService;
    public AdminMenu(UserService userService, LaneService laneService, IAuthService authService, ReservationService reservationService) : base("Admin menu", authService)
    {
        _userService = userService;
        _laneService = laneService;
        _authService = authService;
        _reservationService = reservationService;
        AddMenuOption("Add user", AddUser);
        AddMenuOption("Delete user", ()=>DeleteList(_userService));
        AddMenuOption("List of users", ()=>List(_userService));
        AddMenuOption("Add reservation", AddReservation);
        AddMenuOption("Delete reservation", ()=>DeleteList(_reservationService));
        AddMenuOption("List reservations", ()=>List(_reservationService));
    }

    private void AddReservation()
    {
        var clients = _userService.GetAllByRole(Role.Client);
        if (clients == null || clients.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No clients found.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }
        
        var lanes = _laneService.GetAvailable();
        if (lanes == null || lanes.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No available lanes found.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }

        var client = Prompt.SelectFromList("Choose client", clients);
        var lane = Prompt.SelectFromList("Choose lane", lanes);
        
        string details = Prompt.GetString("Enter reservation details: ");
        DateTime date = Prompt.GetDate("Enter reservation date: ");
        Reservation newReservation = new Reservation(date, details, client.Id, lane.Id);
        _reservationService.Add(newReservation);
        AnsiConsole.MarkupLine("[green]Reservation added successfully.[/]");
        Task.Delay(2000).Wait();
    }

    private void AddUser()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[blue]Creating new user[/]");
            string login = Prompt.GetString("Enter your login:", RegexCheck.IsValidLogin);
            string name = Prompt.GetString("Enter your name:", RegexCheck.IsValidNameAndSurname);
            string surname = Prompt.GetString("Enter your surname:", RegexCheck.IsValidNameAndSurname);
            string password = Prompt.GetString("Enter your password", RegexCheck.IsValidPassword, isSecret: true);
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
            User user = new User(login, name, surname, password, role);
            bool success = _userService.Add(user);
            if (!success)
            {
                AnsiConsole.MarkupLine("[red]User not added.[/]");
                AnsiConsole.MarkupLine("[yellow]Press any key to continue or backspace to go back to menu[/]");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Backspace)
                {
                    return;
                }
            }
            else
            {
                AnsiConsole.MarkupLine("[green]User added successfully.[/]");
                AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                Console.ReadKey();
                return;
            }
        }
    }

    private void DeleteList<T>(IModelService<T> service) where T : IModelType
    {
        string modelName = typeof(T).Name;
        AnsiConsole.Clear();
        var values = service.GetAll();
        if (values == null || values.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Nothing to delete.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
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
            AnsiConsole.MarkupLine($"[green]{modelName}s deleted successfully.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]No {modelName}s deleted.[/]");
        }

        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private void List<T>(IModelService<T> service) where T : IModelType
    {
        AnsiConsole.Clear();
        string modelName = typeof(T).Name;
        var models = service.GetAll();
        AnsiConsole.Markup($"[blue]List of {modelName}s[/]\n");
        if (models == null)
        {
            AnsiConsole.MarkupLine($"[red]No {modelName}s found.[/]");
        }
        else
        {
            var properties = typeof(T).GetProperties();
        
            // Utwórz tabelę i dodaj kolumny dla każdej właściwości
            var table = new Table().Centered();
            AnsiConsole.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Cropping(VerticalOverflowCropping.Bottom)
                .Start(ctx =>
                {
                    foreach (var prop in properties)
                    {
                        // Pomijamy właściwości typu ICollection i podobne
                        if (!prop.PropertyType.IsGenericType && 
                            !typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) || 
                            prop.PropertyType == typeof(string))
                        {
                            table.AddColumn($"[bold cyan1]{prop.Name}[/]");
                            ctx.Refresh();
                            Task.Delay(100).Wait();
                        }
                    }
        
                    // Dodaj dane dla każdego obiektu
                    foreach (var model in models)
                    {
                        var rowData = new List<string>();
                        foreach (var prop in properties)
                        {
                            // Pomijamy kolekcje
                            if (!prop.PropertyType.IsGenericType && 
                                !typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) || 
                                prop.PropertyType == typeof(string))
                            {
                                var value = prop.GetValue(model);
                                rowData.Add(value?.ToString() ?? "");
                            }
                        }
                        table.AddRow(rowData.ToArray());
                        ctx.Refresh();
                        Task.Delay(100).Wait();
                    }

                    table.Border(TableBorder.Heavy);
                });
        }
        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
        Console.ReadKey();
    }
}