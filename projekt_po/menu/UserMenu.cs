using projekt_po.Model;
using projekt_po.Services;
using Spectre.Console;
using projekt_po.Utils;

namespace projekt_po.Menu;

public class UserMenu
{
    private readonly IAuthService _authService;
    private readonly ReservationService _reservationService;

    public UserMenu(IAuthService authService, ReservationService reservationService)
    {
        _authService = authService;
        _reservationService = reservationService;
    }

    public void ShowUserMenu()
    {
        Console.Clear();
        while (true)
        {
            Console.Clear();
            var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title("[red]User menu[/]")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Make reservation",
                    "Your reservations",
                    "Delete reservation",
                    "Logout"
                }));
            switch (option)
            {
                case "Make reservation":
                    MakeReservation();
                    break;
                case "Your reservations":
                    ShowUserReservations();
                    break;
                case "Delete reservation":
                    DeleteReservation();
                    break;
                case "Logout":
                    _authService.Logout();
                    return;
            }
        }
    }

    private void DeleteReservation()
    {
        Console.Clear();
        int userId = _authService.GetLoggedUser()!.Id;
        var reservations = _reservationService.GetAllByUser(userId);
        if (reservations.Count == 0)
        {
            Console.WriteLine("No reservations to delete found.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        var reservationsToDelete = AnsiConsole.Prompt(
            new MultiSelectionPrompt<Reservation>()
                .Title("Choose reservations to delete")
                .NotRequired()
                .PageSize(15)
                .MoreChoicesText("[grey](Move up and down))[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to pick a reservation to delete, " +
                    "[green]<enter>[/] to accept)[/]")
                .AddChoices(reservations)
                .UseConverter(rez => $"reservation details: {rez.Details}, Date: {rez.Date}")
        );

        foreach (var res in reservationsToDelete)
        {
            _reservationService.Delete(res.Id);
        }

        if (reservationsToDelete.Count > 0)
        {
            Console.WriteLine("Reservations deleted successfully.");
        }
        else
        {
            Console.WriteLine("No reservations deleted.");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    private void MakeReservation()
    {
        Console.Clear();
        string details = Prompt.GetString("Enter details about reservation:");
        DateTime reservationDate = default;
        bool dateAvailable = false;

        while (!dateAvailable)
        {
            reservationDate = Prompt.GetDate("Enter date of reservation: ");

            if (reservationDate < DateTime.Now)
            {
                AnsiConsole.WriteLine("Date must be in the future.");
                continue;
            }
            if (!_reservationService.CheckAvailability(reservationDate))
            {
                AnsiConsole.WriteLine("Date is not available.");
                continue;
            }
            
            dateAvailable = true;
        }

        var user = _authService.GetLoggedUser()!;
        _reservationService.Add(new Reservation(reservationDate, details, user.Id));
        AnsiConsole.WriteLine("Reservation added successfully.");
        Task.Delay(2000).Wait();
    }

     private void ShowUserReservations()
     {
         Console.Clear();
         int userId = _authService.GetLoggedUser()!.Id;
         var reservations = _reservationService.GetAllByUser(userId);
         if (reservations.Count == 0)
         {
             Console.WriteLine("No reservations found.");
         }
         else
         {
             foreach (var reservation in reservations)
             {
                 Console.WriteLine(reservation);
             }
         }
         Console.WriteLine("Press any key to continue...");
         Console.ReadKey();
     }
}