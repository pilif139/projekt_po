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
    
    public User User { get; set; } = null!; 

    
    public Reservation() { }

    public Reservation(DateTime date, string details, int userId)
    {
        Date = date;
        Details = details;
        UserId = userId;
    }
}