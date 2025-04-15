﻿using projekt_po.Model;
using projekt_po.Services;
using Spectre.Console;
using projekt_po.Utils;
using Bogus;

namespace projekt_po.Menu;

public class ClientMenu : BaseMenu
{
    private readonly IAuthService _authService;
    private readonly ReservationService _reservationService;
    private readonly LaneService _laneService;

    public ClientMenu(IAuthService authService, ReservationService reservationService, LaneService laneService) : base("Client menu", authService)
    {
        _authService = authService;
        _reservationService = reservationService;
        _laneService = laneService;
        AddMenuOption("Make reservation", MakeReservation);
        AddMenuOption("Your reservations", ShowUserReservations);
        AddMenuOption("Edit reservation", EditReservation);
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

    private void EditReservation()
    {
        Console.Clear();
        int userId = _authService.GetLoggedUser()!.Id;
        var reservations = _reservationService.GetAllByUser(userId);
        if (reservations == null || reservations.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]No reservations to edit found.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }

        var reservationToEdit = Prompt.SelectFromList("Select reservation to edit", reservations);
        AnsiConsole.MarkupLine($"[blue]Selected reservation: {reservationToEdit.Details}[/]");
        string details = Prompt.GetString("Enter new details about reservation or enter to skip this:");
        if (string.IsNullOrEmpty(details) || details.Trim().Length == 0)
        {
            details = reservationToEdit.Details;
        }
        DateTime reservationDate = default;
        Lane? lane = null;
        bool dateAvailable = false;
        while (!dateAvailable)
        {
            reservationDate = Prompt.GetDate("Enter new date of reservation: ");

            if (reservationDate < DateTime.Now)
            {
                AnsiConsole.MarkupLine("[red]Date must be in the future.[/]");
                continue;
            }

            var lanes = _laneService.GetByDate(reservationDate);
            if (lanes == null || lanes.Count == 0) continue;
            lane = Prompt.SelectFromList("Select available lane", lanes);
            dateAvailable = true;
        }
        Reservation updatedReservation = new Reservation(reservationDate, details, reservationToEdit.UserId, lane.Id);
        updatedReservation.Id = reservationToEdit.Id;
        _reservationService.Update(updatedReservation);
    }

    private void MakeReservation()
    {
        Console.Clear();
        string details = Prompt.GetString("Enter details about reservation:");
        DateTime reservationDate = default;
        bool dateAvailable = false;
        Lane? lane = null;

        while (!dateAvailable)
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
                AnsiConsole.MarkupLine("[red]No lanes on this date available.[/]");
                continue;
            }
            lane = Prompt.SelectFromList("Select available lane", lanes);
            dateAvailable = true;
        }

        var user = _authService.GetLoggedUser()!;
        _reservationService.Add(new Reservation(reservationDate, details, user.Id, lane.Id));
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