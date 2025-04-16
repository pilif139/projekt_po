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
        AddMenuOption("Delete user", () => DeleteList(_userService));
        AddMenuOption("Edit user", EditUser);
        AddMenuOption("List of users", () => List(_userService));
        AddMenuOption("Add reservation", AddReservation);
        AddMenuOption("Delete reservation", () => DeleteList(_reservationService));
        AddMenuOption("Edit reservation", EditReservation);
        AddMenuOption("List reservations", () => List(_reservationService));
        AddMenuOption("Add lane", AddLane);
        AddMenuOption("Delete lane", () => DeleteList(_laneService));
        AddMenuOption("List lanes", () => List(_laneService));
    }

    private void EditReservation()
    {
        var reservations = _reservationService.GetAll();
        if (reservations == null || reservations.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No reservations found.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }

        var reservation = Prompt.SelectFromList("Choose reservation", reservations);
        string details = Prompt.GetString("Enter new reservation details: ");
        DateTime date = Prompt.GetDate("Enter new reservation date: ");
        reservation.Details = details;
        reservation.Date = date;
        _reservationService.Update(reservation);
        AnsiConsole.MarkupLine("[green]Reservation updated successfully.[/]");
        Task.Delay(2000).Wait();
    }

    private void EditUser()
    {
        var clients = _userService.GetAllByRole(Role.Client);
        var workers = _userService.GetAllByRole(Role.Worker);
        var users = clients?.Concat(workers!).ToList();
        
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[blue]Editing user[/]");
        if (users == null || users.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No users found to edit.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }
        var user = Prompt.SelectFromList("Choose user to edit", users);
        // from editable properties make a menu to pick which one to edit and then make prompt for each one
        var menuOpitons = new List<string>
        {
            "Login",
            "Name",
            "Surname",
            "Password",
            "Role",
            "Exit"
        };
        bool exit = false;
        while (!exit)
        {
            var selectedOption = Prompt.SelectFromList("Choose property to edit", menuOpitons);
            switch (selectedOption)
            {
                case "Login":
                    Func<string, bool> validateLogin = (val) =>
                    {
                        if (_userService.GetByLogin(val) == null) return true;
                        AnsiConsole.MarkupLine("[red]Login already in use![/]");
                        return false;
                    };
                    string newLogin = Prompt.GetString("Enter new login:", RegexCheck.IsValidLogin, validateLogin);
                    user.Login = newLogin;
                    break;
                case "Name":
                    string newName = Prompt.GetString("Enter new name:", RegexCheck.IsValidNameAndSurname);
                    user.Name = newName;
                    break;
                case "Surname":
                    string newSurname = Prompt.GetString("Enter new surname:", RegexCheck.IsValidNameAndSurname);
                    user.Surname = newSurname;
                    break;
                case "Password":
                    string newPassword = Prompt.GetString("Enter new password:", true, RegexCheck.IsValidPassword);
                    user.Password = Hash.HashPassword(newPassword);
                    break;
                case "Role":
                    Role newRole = Prompt.SelectFromList<Role>("Choose role to edit");
                    user.Role = newRole;
                    break;
                case "Exit":
                    exit = true;
                    break;
            }
        }
        _userService.Update(user);
        Task.Delay(2000).Wait();
    }

    private void AddLane()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[blue]Creating new lane[/]");
        while (true)
        {
            int number = Prompt.GetNumber("Enter lane number: ", 0, 1000);
            decimal price = Prompt.GetNumber("Enter lane price: ", 0m, 1000m);
            var status = Prompt.SelectFromList<LaneStatus>("Select status");
            var workers = _userService.GetAllByRole(Role.Worker);
            if (workers == null || workers.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No workers found.[/]");
                AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                Console.ReadKey();
                return;
            }

            var worker = Prompt.SelectFromList("Select worker", workers);
            Lane newLane = new Lane(status, number, price, worker.Id);
            if (_laneService.Add(newLane))
            {
                AnsiConsole.MarkupLine("[green]Lane added successfully.[/]");
                AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                Console.ReadKey();
                return;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Lane not added.[/]");
            }
        }
    }

    private void AddReservation()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[blue]Creating new reservation[/]");
        var clients = _userService.GetAllByRole(Role.Client);
        if (clients == null || clients.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No clients found.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }
        var client = Prompt.SelectFromList("Choose client", clients);
        DateTime reservationDate = default;
        Lane? lane = null;

        while (true)
        {
            reservationDate = Prompt.GetDate("Enter date of reservation: ");
            if (reservationDate < DateTime.Now)
            {
                AnsiConsole.MarkupLine("[red]Date must be in the future.[/]");
                continue;
            }
            var lanes = _laneService.GetByDate(reservationDate);
            if (lanes == null || lanes.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No lanes available.[/]");
                continue;
            }
            lane = Prompt.SelectFromList("Select available lane", lanes);
            string details = Prompt.GetString("Enter reservation details: ");
            Reservation newReservation = new Reservation(reservationDate, details, client.Id, lane.Id);
            if (_reservationService.Add(newReservation)) break;
        }
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
            string password = Prompt.GetString("Enter your password",true, RegexCheck.IsValidPassword);
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

            var table = new Table().Centered();
            AnsiConsole.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Cropping(VerticalOverflowCropping.Bottom)
                .Start(ctx =>
                {
                    // add columns to the table
                    foreach (var prop in properties)
                    {
                        // properties that are not collections etc
                        if (!prop.PropertyType.IsGenericType &&
                            !typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) ||
                            prop.PropertyType == typeof(string))
                        {
                            table.AddColumn($"[bold cyan1]{prop.Name}[/]");
                            ctx.Refresh();
                            Task.Delay(100).Wait();
                        }
                    }
                    // add rows to the table
                    foreach (var model in models)
                    {
                        var rowData = new List<string>();
                        foreach (var prop in properties)
                        {
                            // same check as above
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
