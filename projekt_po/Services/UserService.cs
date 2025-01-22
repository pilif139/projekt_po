using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Utils;

namespace projekt_po.Services;

public class UserService
{
    private readonly UserRepository _userRepository;
    
    public event LogEventHandler? LogEvent;

    public UserService(UserRepository userRepository, Logger logger)
    {
        _userRepository = userRepository;
        LogEvent += logger.Log;
    }
    
    public void AddUser(string name, string surname, string password, Role role)
    {
        string hashedPassword = Hash.HashPassword(password);
        _userRepository.Add(name, surname, hashedPassword, role);
        Console.WriteLine("User added successfully.");
        LogEvent?.Invoke($"User {name} {surname} with role {role} added.");
    }
    
    public void DeleteUser(int id)
    {
        bool success = _userRepository.Delete(id);
        if (success)
        {
            Console.WriteLine("User deleted successfully.");
            LogEvent?.Invoke($"User with id {id} deleted.");
        }
        else
        {
            Console.WriteLine("User not found.");
            LogEvent?.Invoke($"Tried to delete non-existent user with {id} id.");
        }
    }
    
    public User? GetUserById(int id)
    {
        var user = _userRepository.GetById(id);
        if (user == null)
        {
            Console.WriteLine("User not found");
            LogEvent?.Invoke($"User with id {id} not found.");
            return null;
        }
        LogEvent?.Invoke($"User with id {id} found.");
        return user;
    }

    public void ListUsers()
    {
        var users = _userRepository.GetAll();
        Console.WriteLine("Users:");
        foreach (var user in users)
        {
            Console.WriteLine($"ID:{user.Id}.{user.Name} {user.Surname}, rola: {user.Role}");
        }
        LogEvent?.Invoke("List of users displayed.");
    }
}