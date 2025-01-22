﻿using projekt_po.Database;
using projekt_po.Model;

namespace projekt_po.Repository;

public class UserRepository
{
    private readonly DatabaseContext _db;
    
    public UserRepository(DatabaseContext db)
    {
        _db = db;
    }
    
    public void Add(string name, string surname, string password, Role role)
    {
        _db.Users.Add(new User(name, surname, password, role));
        _db.SaveChanges();
    }
    
    public User? GetById(int id)
    {
        return _db.Users.Find(id);
    }
    
    public List<User> GetAll()
    {
        return _db.Users.ToList();
    }

    public List<User> GetAllByRole(Role role)
    {
        return _db.Users.Where(u => u.Role == role).ToList();
    }
    
    public User? GetByNameAndSurname(string name, string surname)
    {
        return _db.Users.FirstOrDefault(u => u.Name == name && u.Surname == surname);
    }
    
    public void Delete(int id)
    {
        var user = _db.Users.Find(id);
        if (user != null)
        {
            _db.Users.Remove(user);
            _db.SaveChanges();
        }
        else
        {
            Console.WriteLine("User not found");
        }
    }
    
    
}