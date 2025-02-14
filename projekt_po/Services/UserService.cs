using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Utils;

namespace projekt_po.Services;

public class UserService : BaseService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository, ILogger logger) : base(logger)
    {
        _userRepository = userRepository;
    }

    public void AddUser(string name, string surname, string password, Role role)
    {
        string hashedPassword = Hash.HashPassword(password);
        _userRepository.Add(name, surname, hashedPassword, role);
        Console.WriteLine("User added successfully.");
        Log($"User {name} {surname} with role {role} added.");
    }

    public void DeleteUser(int id)
    {
        bool success = _userRepository.Delete(id);
        if (success)
        {
            Console.WriteLine("User deleted successfully.");
            Log($"User with id {id} deleted.");
        }
        else
        {
            Console.WriteLine("User not found.");
            Log($"Tried to delete non-existent user with {id} id.");
        }
    }

    public User? GetUserById(int id)
    {
        var user = _userRepository.GetById(id);
        if (user == null)
        {
            Console.WriteLine("User not found");
            Log($"User with id {id} not found.");
            return null;
        }
        Log($"User with id {id} found.");
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
        Log("List of users displayed.");
    }
}