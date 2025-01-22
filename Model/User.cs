﻿namespace projekt_po.Model;

public class User
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Surname { get; set; }
    public string Password { get; set; }
    public Role Role { get; set; }

    public User(string name, string surname, string password, Role role)
    {
        Name = name;
        Surname = surname;
        Password = password;
        Role = role;
    }
}