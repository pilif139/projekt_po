using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Services;

public class ReservationService : BaseService, IModelService<Reservation>
{
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
        if (!_rbacService.CheckPermission(Resource.Reservation, Permission.Read, reservation)) return null;
        Log($"Getting reservation with id {id}.");
        return reservation;
    }

    public List<Reservation> GetAll()
    {
        if (!_rbacService.CheckPermission(Resource.Reservation, Permission.All)) return new List<Reservation>();
        Log("Getting all reservations.");
        return _reservationRepository.GetAll();
    }

    public List<Reservation>? GetAllByUser(int userId)
    {
        var reservations = _reservationRepository.GetAllByUser(userId);
        if (reservations == null || reservations.Count == 0) return new List<Reservation>();
        if (!_rbacService.CheckPermission(Resource.Reservation, Permission.Read, reservations[0])) return new List<Reservation>();
        Log("Getting all reservations for user with id:" + userId);
        return reservations;
    }

    public void Add(Reservation reservation)
    {
        if (!_rbacService.CheckPermission(Resource.Reservation, Permission.Create)) return;

        // checks if user exists
        var user = _userService.GetById(reservation.UserId);
        if (user == null)
        {
            AnsiConsole.MarkupLine("User not found.");
            Log($"Tried to add reservation for non-existent user with {reservation.UserId} id.");
            return;
        }
        // checks if the user has a client role
        if (user.Role != Role.Client)
        {
            AnsiConsole.MarkupLine("User is not a client.");
            Log($"Tried to add reservation for user with {reservation.UserId} id that is not a client.");
            return;
        }

        // checks if the date is available
        var existingReservation = _reservationRepository.GetByDate(reservation.Date);
        if (existingReservation != null)
        {
            AnsiConsole.MarkupLine("Date is not available.");
            Log($"Tried to add reservation for date {reservation.Date} that is not available.");
            return;
        }
        var newReservation = _reservationRepository.Add(reservation.UserId, reservation.Details, reservation.Date);
        Log($"Reservation with id {newReservation.Id} added.");
    }

    public void Delete(int reservationId)
    {
        var reservation = _reservationRepository.Get(reservationId);
        if (reservation == null)
        {
            AnsiConsole.MarkupLine("Reservation not found.");
            Log($"Tried to delete non-existent reservation with {reservationId} id.");
            return;
        }
        if (!_rbacService.CheckPermission(Resource.Reservation, Permission.Delete, reservation)) return;
        bool success = _reservationRepository.Delete(reservationId);
        AnsiConsole.MarkupLine($"Reservation with id {reservationId} deleted successfully.");
        Log($"Reservation with id {reservationId} deleted.");
    }

    public bool CheckAvailability(DateTime date)
    {
        _rbacService.CheckPermission(Resource.Reservation, Permission.Read);
        var reservation = _reservationRepository.GetByDate(date);
        Log("Checked availability for date " + date);
        return reservation == null;
    }

}