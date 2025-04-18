using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using projekt_po.Services;

namespace projekt_po.Model;

public enum LaneStatus
{
    Available,
    Maintenance,
    Cleaning,
    Closed
}

/**
 * Represents a Lane in a system that has one worker assigned at the time and can have many reservations.
 **/
public class Lane : IModelType
{
    [Key]
    public int Id { get; set; }
    
    [Required]
    [Range(1, 1000)]
    public int Number { get; set; }
    
    [Required]
    public LaneStatus Status { get; set; }
    
    [Required]
    [DataType(DataType.Currency)]
    [Range(0, 1000)]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    
    [Required]
    [ForeignKey("User")]
    public int UserId { get; set; }

    public User? User { get; set; }
    
    // [NotMapped]
    // public string FormattedPrice => Price.ToString("C", CultureInfo.CurrentCulture);

    public List<Reservation> Reservations { get; set ; } = new List<Reservation>();

    public Lane()
    {
        Status = LaneStatus.Available;
        Number = 1;
        Price = 0;
    }

    public Lane(LaneStatus status, int number, decimal price)
    {
        Status = status;
        Number = number;
        Price = price;
    }
    
    public Lane(LaneStatus status, int number, decimal price, int userId) : this(status, number, price)
    {
        UserId = userId;
    }
    
    public Lane(LaneStatus status, int number, decimal price, int userId, User user) : this(status, number, price, userId)
    {
        User = user;
    }

    public override string ToString()
    {
        return
            $"Lane number: {Number}, Price: {Price}, status: {Status}, worker assigned: {(User != null ? User.Name + " " + User.Surname : "No worker assigned")}";
    }
}