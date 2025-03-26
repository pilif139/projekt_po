using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Services;

public class ReservationService : BaseService
{
    private readonly IReservationRepository _reservationRepository;
    private readonly IRbacService _rbacService;
    
    public ReservationService(IReservationRepository reservationRepository, IRbacService rbacService, ILogger logger) : base(logger)
    {
        _reservationRepository = reservationRepository;
        _rbacService = rbacService;
    }
    
    public List<Reservation> GetReservations(int userId)
    {
        if(!_rbacService.CheckPermissions(Permissions.Read)) return new List<Reservation>();
        Log("Getting all reservations.");
        return _reservationRepository.GetAll(userId);
    }
    
    public Reservation Add(int userId, string details, DateTime reservationDate)
    {
        if(!_rbacService.CheckPermissions(Permissions.Write)) return new Reservation();
        var reservation = _reservationRepository.Add(userId, details, reservationDate);
        Log($"Reservation with id {reservation.Id} added.");
        return reservation;
    }

    public void Delete(int reservationId)
    {
        if(!_rbacService.CheckPermissions(Permissions.Delete)) return;
        bool success = _reservationRepository.Delete(reservationId);
        if (success)
        {
            AnsiConsole.WriteLine($"Reservation with id {reservationId} deleted successfully.");
            Log($"Reservation with id {reservationId} deleted.");
        }
        else
        {
            AnsiConsole.WriteLine("Reservation not found.");
            Log($"Tried to delete non-existent reservation with {reservationId} id.");
        }
    }
    
    public bool CheckAvailability(DateTime date)
    {
        _rbacService.CheckPermissions(Permissions.Read);
        var reservation = _reservationRepository.GetByDate(date);
        Log("Checked availability for date " + date);
        return reservation == null;
    }
    
}