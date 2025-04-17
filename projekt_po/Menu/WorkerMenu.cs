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
        AnsiConsole.WriteLine("[yellow]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private void UpdateLanePrice()
    {
        AnsiConsole.Clear();
        AnsiConsole.MarkupLine("[cyan]Update lane price[/]");
        var lanes = _laneService.GetAvailable();
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
        AnsiConsole.WriteLine("[yellow]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private void ChangeLaneStatus()
    {
        var lanes = _laneService.GetAll();
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
        lane.Status = status;
        _laneService.Update(lane);
        AnsiConsole.WriteLine("[yellow]Press any key to continue...[/]");
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
        
    }

    private void AddReservation()
    {
        
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
            var reservationsToDelete = Prompt.SelectMultipleFromList("Select reservation to delete", reservations);
            DeleteItems(_reservationService, reservationsToDelete);
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