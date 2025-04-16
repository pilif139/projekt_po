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
                AnsiConsole.MarkupLine("[red]No reservations found.[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[green]Reservations:[/]");
                var table = new Table();
                AnsiConsole.Live(table)
                    .AutoClear(false)
                    .Start(ctx =>
                    {
                        table.AddColumn("Id");
                        ctx.Refresh();
                        Task.Delay(100).Wait();
                        table.AddColumn("Date");
                        ctx.Refresh();
                        Task.Delay(100).Wait();
                        table.AddColumn("Details");
                        ctx.Refresh();
                        Task.Delay(100).Wait();
                        table.AddColumn("Lane Id");
                        ctx.Refresh();
                        Task.Delay(100).Wait();
                        table.AddColumn("User Id");

                        foreach (var reservation in reservations)
                        {
                            table.AddRow(reservation.Id.ToString(), reservation.Date.ToString(), reservation.Details, reservation.LaneId.ToString(), reservation.UserId.ToString());
                            ctx.Refresh();
                            Task.Delay(100).Wait();
                        }
                        table.Border(TableBorder.HeavyHead);
                        table.Border(TableBorder.Rounded);
                        ctx.Refresh();
                        Task.Delay(100).Wait();
                    });
            }
        }
        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
        Console.ReadKey();
    }

    private void ShowAllReservations()
    {
        var reservations = _reservationService.GetAll();
        AnsiConsole.Clear();
        if (reservations == null || reservations.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No reservations found.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }
        AnsiConsole.MarkupLine("[green]Reservations:[/]");
        var table = new Table();
        AnsiConsole.Live(table)
            .AutoClear(false)
            .Start(ctx =>
            {
                table.AddColumn("Id");
                ctx.Refresh();
                Task.Delay(100).Wait();
                table.AddColumn("Date");
                ctx.Refresh();
                Task.Delay(100).Wait();
                table.AddColumn("Details");
                ctx.Refresh();
                Task.Delay(100).Wait();
                table.AddColumn("Lane Id");
                ctx.Refresh();
                Task.Delay(100).Wait();
                table.AddColumn("User Id");

                foreach (var reservation in reservations)
                {
                    table.AddRow(reservation.Id.ToString(), reservation.Date.ToString(), reservation.Details, reservation.LaneId.ToString(), reservation.UserId.ToString());
                    ctx.Refresh();
                    Task.Delay(100).Wait();
                }
                table.Border(TableBorder.HeavyHead);
                table.Border(TableBorder.Rounded);
                ctx.Refresh();
                Task.Delay(100).Wait();
            });
        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
        Console.ReadKey();
    }
}