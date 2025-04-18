using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using projekt_po.Services;

namespace projekt_po.Model;

public class Reservation : IModelType
{
    [Key]
    public int Id { get; set; }

    [Required]
    public DateTime Date { get; set; }

    [Required]
    [StringLength(100)]
    public string Details { get; set; }

    [ForeignKey("User")]
    public int UserId { get; set; }
    public User? User { get; set; }
    
    [ForeignKey("Lane")]
    public int LaneId { get; set; }
    public Lane? Lane { get; set; }

    public Reservation()
    {
        Details = string.Empty;
        Date = DateTime.Now;
    }

    public Reservation(DateTime date, string details, int userId, int laneId)
    {
        Date = date;
        Details = details;
        UserId = userId;
        LaneId = laneId;
    }

    public Reservation(DateTime date, string details, int userId, User user, int laneId, Lane lane) : this(date, details, userId, laneId)
    {
        User = user;
        Lane = lane;
    }         
    

    public override string ToString()
    {
        // Assuming the issue is with User or Lane properties being null
        string userInfo = User != null ? $"{User.Name} {User.Surname}" : "Unknown User";
        string laneInfo = Lane != null ? $"{Lane.Number}, {Lane.Price}" : "Unknown Lane";
    
        return $"Reservation on {Date:yyyy-MM-dd HH:mm} for user: {userInfo} on lane {laneInfo} - {Details}";
    }
}