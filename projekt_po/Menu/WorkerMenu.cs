using projekt_po.Menu;
using projekt_po.Services;
using Spectre.Console;

public class WorkerMenu : BaseMenu
{
    private readonly IAuthService _authService;
    private readonly ReservationService _reservationService;
    private readonly UserService _userService;
    
    public WorkerMenu(IAuthService authService, ReservationService reservationService, UserService userService) : base("Worker menu", authService)
    {
        _authService = authService;
        _reservationService = reservationService;
        _userService = userService;
        AddMenuOption("Show all reservations", ShowAllReservations);
        AddMenuOption("Show user reservations", ShowUserReservations);
        AddMenuOption("Delete reservation", DeleteReservation);
        AddMenuOption("Update reservation", UpdateReservation);
        AddMenuOption("Add reservation", AddReservation);
        AddMenuOption("Show your lanes", ShowLanes);
        AddMenuOption("Change lane status", ChangeLaneStatus);
        AddMenuOption("Update lane price", UpdateLanePrice);
    }

    private void UpdateLanePrice()
    {
        throw new NotImplementedException();
    }

    private void ChangeLaneStatus()
    {
        throw new NotImplementedException();
    }

    private void ShowLanes()
    {
        throw new NotImplementedException();
    }

    private void UpdateReservation()
    {
        throw new NotImplementedException();
    }

    private void AddReservation()
    {
        throw new NotImplementedException();
    }

    private void DeleteReservation()
    {
        throw new NotImplementedException();
    }

    private void ShowUserReservations()
    {
        throw new NotImplementedException();
    }

    private void ShowAllReservations()
    {
        var reservations = _reservationService.GetAll();
        if (reservations == null || reservations.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No reservations found.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }
        
    }
}