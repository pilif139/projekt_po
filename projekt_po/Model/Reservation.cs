using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace projekt_po.Model;

public class Reservation
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    public DateTime Date { get; set; }
    
    [Required]
    public string Details { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User User { get; set; }

    public Reservation()
    {
        Details = string.Empty;
        User = new User();
    }

    public Reservation(DateTime date, string details, int userId)
    {
        Date = date;
        Details = details;
        UserId = userId;
        User = new User();
    }

    public Reservation(DateTime date, string details, int userId, User user) : this(date, details, userId)
    {
        User = user;
    }
    
    public override string ToString()
    {
        return $"Id: {Id}, Date: {Date}, Details: {Details}, UserId: {UserId}";
    }
}