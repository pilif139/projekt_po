using projekt_po.Model;
using projekt_po.Database;

namespace projekt_po.Repository;

public class ReservationRepository
{
    private readonly DatabaseContext _db;

    public ReservationRepository(DatabaseContext db) 
    {
        _db = db;
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
        Console.WriteLine("Reservation added successfully");
        return reservation;
    }
}