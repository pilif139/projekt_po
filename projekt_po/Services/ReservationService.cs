using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Services;

public class ReservationService : BaseService, IModelService<Reservation>
{
    private const Resource Reservation = Resource.Reservation;
    private readonly IReservationRepository _reservationRepository;
    private readonly UserService _userService;
    private readonly IRbacService _rbacService;

    public ReservationService(IReservationRepository reservationRepository, UserService userService, IRbacService rbacService, ILogger logger) : base(logger)
    {
        _reservationRepository = reservationRepository;
        _rbacService = rbacService;
        _userService = userService;
    }

    public Reservation? GetById(int id)
    {
        var reservation = _reservationRepository.Get(id);
        if (reservation == null)
        {
            AnsiConsole.MarkupLine("Reservation not found.");
            Log($"Tried to get non-existent reservation with {id} id.");
            return null;
        }
        if (!_rbacService.CheckPermission(Reservation, Permission.Read, reservation)) return null;
        Log($"Getting reservation with id {id}.");
        return reservation;
    }

    public List<Reservation>? GetAll()
    {
        if (!_rbacService.CheckPermission(Reservation, Permission.All)) return new List<Reservation>();
        Log("Getting all reservations.");
        return _reservationRepository.GetAll();
    }

    public List<Reservation>? GetAllByUser(int userId)
    {
        var reservations = _reservationRepository.GetAllByUser(userId);
        if (reservations == null || reservations.Count == 0) return new List<Reservation>();
        if (!_rbacService.CheckPermission(Reservation, Permission.Read, reservations[0])) return new List<Reservation>();
        Log("Getting all reservations for user with id:" + userId);
        return reservations;
    }

    public bool Add(Reservation reservation)
    {
        if (!_rbacService.CheckPermission(Reservation, Permission.Create)) return false;
        // checks if user exists
        var user = _userService.GetById(reservation.UserId);
        if (user == null)
        {
            AnsiConsole.MarkupLine("User not found.");
            Log($"Tried to add reservation for non-existent user with {reservation.UserId} id.");
            return false;
        }
        // checks if the user has a client role
        if (user.Role != Role.Client)
        {
            AnsiConsole.MarkupLine("User is not a client.");
            Log($"Tried to add reservation for user with {reservation.UserId} id that is not a client.");
            return false;
        }

        // checks if the date is available
        bool isAvailable = CheckAvailability(reservation.Date, reservation.LaneId);
        if (!isAvailable)
        {
            AnsiConsole.MarkupLine("Date is not available.");
            Log($"Tried to add reservation for date {reservation.Date} that is not available.");
            return false;
        }
        var newReservation = _reservationRepository.Add(reservation.UserId, reservation.Details, reservation.Date);
        Log($"Reservation with id {newReservation.Id} added.");
        return true;
    }

    public bool Delete(int reservationId)
    {
        var reservation = _reservationRepository.Get(reservationId);
        if (reservation == null)
        {
            AnsiConsole.MarkupLine("Reservation not found.");
            Log($"Tried to delete non-existent reservation with {reservationId} id.");
            return false;
        }
        if (!_rbacService.CheckPermission(Reservation, Permission.Delete, reservation)) return false;
        bool success = _reservationRepository.Delete(reservationId);
        if (success)
        {
            AnsiConsole.MarkupLine($"Reservation with id {reservationId} deleted successfully.");
            Log($"Reservation with id {reservationId} deleted.");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to delete reservation.[/]");
            Log($"Failed to delete reservation with id {reservationId}.");
        }
        return success;
    }

    public bool Update(Reservation reservation)
    {
        var existingReservation = _reservationRepository.Get(reservation.Id);
        if (existingReservation == null)
        {
            AnsiConsole.MarkupLine("[red]Reservation not found.[/]");
            Log($"Tried to update non-existent reservation with {reservation.Id} id.");
            return false;
        }
        if (!_rbacService.CheckPermission(Reservation, Permission.Update, existingReservation)) return false;
        // checks if the date is available
        if (!CheckAvailability(reservation.Date, reservation.LaneId))
        {
            AnsiConsole.MarkupLine("[red]Date is not available.[/]");
            Log($"Tried to update reservation for date {reservation.Date} that is not available.");
            return false;
        }
        if (reservation.UserId != existingReservation.UserId &&
           _userService.GetById(reservation.UserId) == null)
        {
            AnsiConsole.MarkupLine("[red]User not found.[/]");
            Log($"Tried to update reservation for non-existent user with {reservation.UserId} id.");
            return false;
        }

        bool success = _reservationRepository.Update(reservation);
        if (success)
        {
            Log($"Reservation with id {reservation.Id} updated.");
            AnsiConsole.MarkupLine($"[green]Reservation with id {reservation.Id} updated successfully.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine("[red]Failed to update reservation.[/]");
            Log($"Failed to update reservation with id {reservation.Id}.");
        }
        return success;
    }

    public bool CheckAvailability(DateTime date, int laneId)
    {
        _rbacService.CheckPermission(Reservation, Permission.Read);
        var reservation = _reservationRepository.GetByDate(date);
        if (reservation != null && reservation.LaneId == laneId)
        {
            AnsiConsole.MarkupLine("[red]Date is not available.[/]");
            Log($"Tried to check availability for date {date} on lane with id: {laneId} that is not available.");
            return false;
        }
        AnsiConsole.MarkupLine("[green]Availability available.[/]");
        Log("Checked availability for available date " + date + " and lane " + laneId);
        return reservation == null;
    }

}