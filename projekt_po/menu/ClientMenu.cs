using projekt_po.Model;
using projekt_po.Services;
using Spectre.Console;
using projekt_po.Utils;

namespace projekt_po.Menu;

public class ClientMenu : BaseMenu
{
    private readonly IAuthService _authService;
    private readonly ReservationService _reservationService;

    public ClientMenu(IAuthService authService, ReservationService reservationService) : base("Client menu",authService)
    {
        _authService = authService;
        _reservationService = reservationService;
        AddMenuOption("Make reservation", MakeReservation);
        AddMenuOption("Your reservations", ShowUserReservations);
        AddMenuOption("Delete reservation", DeleteReservation);
    }
    
    private void DeleteReservation()
    {
        Console.Clear();
        int userId = _authService.GetLoggedUser()!.Id;
        var reservations = _reservationService.GetAllByUser(userId);
        if (reservations == null || reservations.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No reservations to delete found.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
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
                .UseConverter(rez => $"reservation details:\n - {rez.Details},\n Date: {rez.Date}\n")
        );

        foreach (var res in reservationsToDelete)
        {
            _reservationService.Delete(res.Id);
        }

        if (reservationsToDelete.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Deleted 0 reservations.[/]");
        }

        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
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
                AnsiConsole.MarkupLine("[red]Date must be in the future.[/]");
                continue;
            }
            if (!_reservationService.CheckAvailability(reservationDate))
            {
                AnsiConsole.MarkupLine("[red]Date is not available.[/]");
                continue;
            }

            dateAvailable = true;
        }

        var user = _authService.GetLoggedUser()!;
        _reservationService.Add(new Reservation(reservationDate, details, user.Id));
        AnsiConsole.MarkupLine("[green]Reservation added successfully.[/]");
        Task.Delay(2000).Wait();
    }

    private void ShowUserReservations()
    {
        Console.Clear();
        int userId = _authService.GetLoggedUser()!.Id;
        var reservations = _reservationService.GetAllByUser(userId);
        if (reservations == null || reservations.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No reservations found.[/]");
        }
        else
        {
            var table = new Table();
            AnsiConsole.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Start(ctx =>
                {
                    table.AddColumn("[green bold]Id[/]");
                    ctx.Refresh();
                    Task.Delay(100).Wait();
                    table.AddColumn("[dodgerblue2 bold]Details[/]");
                    ctx.Refresh();
                    Task.Delay(100).Wait();
                    table.AddColumn("[blueviolet]Date[/]");
                    ctx.Refresh();
                    Task.Delay(100).Wait();

                    foreach (var reservation in reservations)
                    {
                        table.AddRow(reservation.Id.ToString(), reservation.Details, reservation.Date.ToString("yyyy-MM-dd HH:mm:ss"));
                        ctx.Refresh();
                        Task.Delay(100).Wait();
                    }
                    
                    table.Border(TableBorder.Rounded);
                    ctx.Refresh();
                    Task.Delay(100).Wait();
                });
        }
        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
        Console.ReadKey();
    }
}