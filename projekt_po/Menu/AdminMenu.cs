using projekt_po.Model;
using projekt_po.Services;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Menu;

public class AdminMenu : BaseMenu
{
    private readonly UserService _userService;
    private readonly LaneService _laneService;
    private readonly ReservationService _reservationService;

    public AdminMenu(UserService userService, LaneService laneService, IAuthService authService,
        ReservationService reservationService) : base("Admin menu", authService)
    {
        _userService = userService;
        _laneService = laneService;
        _reservationService = reservationService;
        AddMenuOption("Add user", AddUser);
        AddMenuOption("Delete user", () => DeleteItems(_userService, _userService.GetAll()));
        AddMenuOption("Edit user", EditUser);
        AddMenuOption("List of users", () => ListItems(_userService.GetAll()));
        AddMenuOption("Add reservation", AddReservation);
        AddMenuOption("Delete reservation", () => DeleteItems(_reservationService, _reservationService.GetAll()));
        AddMenuOption("Edit reservation", EditReservation);
        AddMenuOption("List reservations", () => ListItems(_reservationService.GetAll()));
        AddMenuOption("Add lane", AddLane);
        AddMenuOption("Delete lane", () => DeleteItems(_laneService, _laneService.GetAll()));
        AddMenuOption("Edit lane", EditLane);
        AddMenuOption("List lanes", () => ListItems(_laneService.GetAll()));
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
        var menuOptions = new List<string>
        {
            "Date",
            "Details",
            "Lane",
            "Exit"
        };
        bool exit = false;
        while (!exit)
        {
            var option = Prompt.SelectFromList("Choose property to edit", menuOptions);
            switch (option)
            {
                case "Date":
                    Func<DateTime, bool> validateDate = (val) =>
                    {
                        if (val < DateTime.Now)
                        {
                            AnsiConsole.MarkupLine("[red]Date must be in the future.[/]");
                            return false;
                        }

                        if (!_reservationService.CheckAvailability(val, reservation.LaneId))
                        {
                            AnsiConsole.MarkupLine("[red]Lane is not available on this date.[/]");
                            return false;
                        }
                        return true;
                    };
                    DateTime newDate = Prompt.GetDate("Enter new date:", validateDate);
                    reservation.Date = newDate;
                    break;
                case "Details":
                    string newDetails = Prompt.GetString("Enter new details:");
                    reservation.Details = newDetails;
                    break;
                case "Lane":
                    var lanes = _laneService.GetByDate(reservation.Date);
                    var lane = Prompt.SelectFromList("Select available lane", lanes!);
                    reservation.LaneId = lane.Id;
                    break;
                case "Exit":
                    exit = true;
                    break;
            }
        }
        _reservationService.Update(reservation);
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

    private void EditLane()
    {
        var lanes = _laneService.GetAll();
        if (lanes == null || lanes.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No lanes found to edit.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[blue]Editing lane[/]");
        var lane = Prompt.SelectFromList("Choose lane to edit", lanes);
        var properties = new List<string>
        {
            "Number",
            "Price",
            "Status",
            "Worker",
            "Exit"
        };
        
        bool exit = false;
        while (!exit)
        {
            var selectedOption = Prompt.SelectFromList("Choose property to edit", properties);
            switch (selectedOption)
            {
                case "Number":
                    int newNumber = Prompt.GetNumber("Enter new lane number: ", 0, 1000);
                    lane.Number = newNumber;
                    break;
                case "Price":
                    decimal newPrice = Prompt.GetNumber("Enter new lane price: ", 0m, 1000m);
                    lane.Price = newPrice;
                    break;
                case "Status":
                    var status = Prompt.SelectFromList<LaneStatus>("Select status");
                    if (status == LaneStatus.Closed) _laneService.CloseLane(lane.Id);
                    lane.Status = status;
                    break;
                case "Worker":
                    var workers = _userService.GetAllByRole(Role.Worker);
                    if (workers == null || workers.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]No workers found.[/]");
                        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                        Console.ReadKey();
                        return;
                    }

                    var worker = Prompt.SelectFromList("Select worker", workers);
                    lane.UserId = worker.Id;
                    break;
                case "Exit":
                    exit = true;
                    break;
            }
        }
        _laneService.Update(lane);
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
            string password = Prompt.GetString("Enter your password", true, RegexCheck.IsValidPassword);
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
                AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                Console.ReadKey();
                return;
            }
        }
    }
}
