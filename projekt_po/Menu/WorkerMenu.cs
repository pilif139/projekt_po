using projekt_po.Menu;
using projekt_po.Model;
using projekt_po.Services;
using projekt_po.Utils;
using Spectre.Console;

public class WorkerMenu : BaseMenu
{
    private readonly IAuthService _authService;
    private readonly ReservationService _reservationService;
    private readonly UserService _userService;
    private readonly LaneService _laneService;

    public WorkerMenu(IAuthService authService, ReservationService reservationService, UserService userService, LaneService laneService) : base("Worker menu", authService)
    {
        _authService = authService;
        _reservationService = reservationService;
        _userService = userService;
        _laneService = laneService;
        AddMenuOption("Show all reservations", ShowAllReservations);
        AddMenuOption("Show user reservations", ShowUserReservations);
        AddMenuOption("Add reservation", AddReservation);
        AddMenuOption("Check date availability", CheckDateAvailability);
        AddMenuOption("Delete reservation", DeleteReservation);
        AddMenuOption("Update reservation", UpdateReservation);
        AddMenuOption("Show your lanes", ShowLanes);
        AddMenuOption("Change lane status", ChangeLaneStatus);
        AddMenuOption("Update lane price", UpdateLanePrice);
    }

    private void CheckDateAvailability()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[cyan]Check date availability[/]");
        var lanes = _laneService.GetAvailable();
        if (lanes == null || lanes.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No available lanes found.[/]");
        }
        else
        {
            var date = Prompt.GetDate("Select date: ");
            var availableLanes = _laneService.GetByDate(date);
            if (availableLanes == null || availableLanes.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No available lanes found for this date.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[green]Available lanes:[/]");
            }
            ListItems(availableLanes);
        }
        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private void UpdateLanePrice()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[cyan]Update lane price[/]");
        var workerId = _authService.GetLoggedUser()!.Id;
        var lanes = _laneService.GetByWorker(workerId);
        if (lanes == null || lanes.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No available lanes found.[/]");
        }
        else
        {
            var lane = Prompt.SelectFromList("Select lane", lanes);
            var newPrice = Prompt.GetNumber("Enter new price", 0m, 1000m);
            lane.Price = newPrice;
            _laneService.Update(lane);
        }
        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private void ChangeLaneStatus()
    {
        var workerId = _authService.GetLoggedUser()!.Id;
        var lanes = _laneService.GetByWorker(workerId);
        if (lanes == null || lanes.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No lanes found.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[cyan]Change lane status[/]");
        var lane = Prompt.SelectFromList("Select lane", lanes);
        var status = Prompt.SelectFromList<LaneStatus>("Select status");
        if (status == LaneStatus.Closed)
        {
            AnsiConsole.MarkupLine("[red]Only admin can close lane.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }
        lane.Status = status;
        _laneService.Update(lane);
        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private void ShowLanes()
    {
        var user = _authService.GetLoggedUser()!;
        var lanes = _laneService.GetByWorker(user.Id);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[cyan]Lanes assigned to you: [/]");
        ListItems(lanes);
    }

    private void UpdateReservation()
    {
        var clients = _userService.GetAllByRole(Role.Client);
        AnsiConsole.Clear();
        if (clients == null || clients.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No clients found.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }
        var client = Prompt.SelectFromList("Select client", clients);
        var reservations = _reservationService.GetAllByUser(client.Id);
        if (reservations == null || reservations.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No reservations found for this client.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }
        var reservation = Prompt.SelectFromList("Select reservation", reservations);
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
                    if (lanes == null || lanes.Count == 0)
                    {
                        AnsiConsole.MarkupLine("[red]No available lanes found for this date.[/]");
                        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                        Console.ReadKey();
                        return;
                    }
                    var lane = Prompt.SelectFromList("Select available lane", lanes);
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

    private void AddReservation()
    {
        var clients = _userService.GetAllByRole(Role.Client);
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[cyan]Add reservation[/]");
        if (clients == null || clients.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No clients found.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }
        var client = Prompt.SelectFromList("Select client", clients);
        while (true)
        {
            var date = Prompt.GetDate("Enter date");
            var lanes = _laneService.GetByDate(date);
            if (lanes == null || lanes.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No available lanes found for this date.[/]");
                AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                Console.ReadKey();
                return;
            }

            var lane = Prompt.SelectFromList("Select lane", lanes);
            var details = Prompt.GetString("Enter details");
            var reservation = new Reservation
            {
                UserId = client.Id,
                LaneId = lane.Id,
                Date = date,
                Details = details
            };
            if (_reservationService.Add(reservation)) break;
        }
        Task.Delay(2500).Wait();
    }

    private void DeleteReservation()
    {
        var clients = _userService.GetAllByRole(Role.Client);
        AnsiConsole.Clear();
        if (clients == null || clients.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No clients found.[/]");
        }
        else
        {
            var client = Prompt.SelectFromList("Select client", clients);
            var reservations = _reservationService.GetAllByUser(client.Id);
            if (reservations == null || reservations.Count == 0)
            {
                AnsiConsole.MarkupLine("[red]No reservations found for this client.[/]");
                AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
                Console.ReadKey();
                return;
            }
            DeleteItems(_reservationService, reservations);
        }
    }

    private void ShowUserReservations()
    {
        var clients = _userService.GetAllByRole(Role.Client);
        AnsiConsole.Clear();
        if (clients == null || clients.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No clients found.[/]");
        }
        else
        {
            var client = Prompt.SelectFromList("Select client", clients);
            var reservations = _reservationService.GetAllByUser(client.Id);
            ListItems(reservations);
        }
    }

    private void ShowAllReservations()
    {
        var reservations = _reservationService.GetAll();
        ListItems(reservations);
    }
}