namespace projekt_po.Services;

public class ReservationService
{
    public int Id { get; set; } 
    public int UserId { get; set; }
    public string Details { get; set; }
    public DateTime ReservationDate { get; set; }
    public DateTime DateOfReservation { get; set; }

    
    public ReservationService() { }

    public ReservationService(int userId, string details, DateTime reservationDate, DateTime dateOfReservation)
    {
        UserId = userId;
        Details = details;
        ReservationDate = reservationDate;
        DateOfReservation = dateOfReservation;
    }
    
}