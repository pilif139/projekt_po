using projekt_po.Model;
using projekt_po.Database;

namespace projekt_po.Repository;

public interface IReservationRepository
{
    Reservation? Get(int reservationId);
    List<Reservation>? GetAll();
    List<Reservation>? GetAllByUser(int userId);
    List<Reservation>? GetByDate(DateTime date);
    List<Reservation>? GetByLane(int laneId);
    Reservation Add(int userId, string details, DateTime reservationDate);
    bool Delete(int reservationId);
    bool Update(Reservation reservation);
}

public class ReservationRepository : IReservationRepository
{
    private readonly DatabaseContext _db;

    public ReservationRepository(DatabaseContext db)
    {
        _db = db;
    }

    public Reservation? Get(int reservationId)
    {
        return _db.Reservations.Find(reservationId);
    }

    public List<Reservation> GetAllByUser(int userId)
    {
        return _db.Reservations
            .Where(r => r.UserId == userId)
            .OrderBy(r => r.Date)
            .ToList();
    }
    public List<Reservation> GetAll()
    {
        return _db.Reservations.OrderBy(r => r.Date).ToList();
    }

    public List<Reservation>? GetByDate(DateTime date)
    {
        return _db.Reservations.Where(r => r.Date == date).ToList();
    }
    
    public List<Reservation> GetByLane(int laneId)
    {
        return _db.Reservations
            .Where(r => r.LaneId == laneId)
            .OrderBy(r => r.Date)
            .ToList();
    }

    public Reservation Add(int userId, string details, DateTime reservationDate)
    {
        var reservation = new Reservation
        {
            UserId = userId,
            Details = details,
            Date = reservationDate
        };

        _db.Reservations.Add(reservation);
        _db.SaveChanges();
        return reservation;
    }

    public bool Delete(int reservationId)
    {
        var reservation = _db.Reservations.Find(reservationId);
        if (reservation == null)
        {
            return false;
        }
        _db.Reservations.Remove(reservation);
        _db.SaveChanges();

        return true;
    }

    public bool Update(Reservation reservation)
    {
        var existingReservation = _db.Reservations.Find(reservation.Id);
        if (existingReservation == null)
        {
            return false;
        }
        existingReservation.UserId = reservation.UserId;
        existingReservation.Details = reservation.Details;
        existingReservation.Date = reservation.Date;
        existingReservation.LaneId = reservation.LaneId;
        _db.Reservations.Update(existingReservation);
        _db.SaveChanges();
        return true;
    }
}