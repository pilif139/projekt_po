using System.ComponentModel.DataAnnotations;

namespace projekt_po.Model;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
    public List<Reservation> Reservations { get; set; } = new List<Reservation>();

    public User(string name, string surname, string password, Role role)
    {
        Name = name;
        Surname = surname;
        Password = password;
        Role = role;
    }

    public User()
    {
        Name = string.Empty;
        Surname = string.Empty;
        Password = string.Empty;
    }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, Surname: {Surname}, Role: {Role}";
    }
}