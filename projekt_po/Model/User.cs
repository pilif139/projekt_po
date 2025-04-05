using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using projekt_po.Services;

namespace projekt_po.Model;

public class User : IModelType
{
    [Key]
    public int Id { get; set; }

    public string Login { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }
    public List<Reservation> Reservations { get; set; } = new List<Reservation>();

    public User(string login,string name, string surname, string password, Role role)
    {
        Login = login;
        Name = name;
        Surname = surname;
        Password = password;
        Role = role;
    }

    public User()
    {
        Login = string.Empty;
        Name = string.Empty;
        Surname = string.Empty;
        Password = string.Empty;
    }

    public override string ToString()
    {
        return $"ID: {Id}, Login: {Login}, Name: {Name}, Surname: {Surname}, Role: {Role}";
    }
}