using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Services;

public class UserService : BaseService, IModelService<User>
{
    private readonly IUserRepository _userRepository;
    private readonly IRbacService _rbacService;
    public UserService(IUserRepository userRepository, IRbacService rbacService, ILogger logger) : base(logger)
    {
        _userRepository = userRepository;
        _rbacService = rbacService;
    }


    public void Add(User user)
    {
        if (!_rbacService.CheckPermission(Resource.User, Permission.Create)) return;
        string hashedPassword = Hash.HashPassword(user.Password);
        _userRepository.Add(user.Name, user.Surname, hashedPassword, user.Role);
        AnsiConsole.WriteLine("User added successfully.");
        Log($"User {user.Name} {user.Surname} with role {user.Role} added.");
    }

    public void Delete(int id)
    {
        if (!_rbacService.CheckPermission(Resource.User, Permission.Delete)) return;
        bool success = _userRepository.Delete(id);
        if (success)
        {
            AnsiConsole.WriteLine($"User with id {id} deleted successfully.");
            Log($"User with id {id} deleted.");
        }
        else
        {
            AnsiConsole.WriteLine("User not found.");
            Log($"Tried to delete non-existent user with {id} id.");
        }
    }

    public User? GetById(int id)
    {
        if (!_rbacService.CheckPermission(Resource.User, Permission.Read)) return null;
        var user = _userRepository.GetById(id);
        if (user == null)
        {
            AnsiConsole.WriteLine("User not found");
            Log($"User with id {id} not found.");
            return null;
        }
        Log($"User with id {id} found.");
        return user;
    }

    public List<User>? GetAll()
    {
        if (!_rbacService.CheckPermission(Resource.User, Permission.Read)) return null;
        var users = _userRepository.GetAll();
        if (users.Count == 0)
        {
            AnsiConsole.WriteLine("No users found.");
            Log("GetAllUsers called, but no users found.");
            return null;
        }
        Log($"GetAllUsers called and found ${users.Count} users.");
        return users;
    }

    public List<User>? GetAllByRole(Role role)
    {
        if (!_rbacService.CheckPermission(Resource.User, Permission.Read)) return null;
        var users = _userRepository.GetAllByRole(role);
        if (users.Count == 0)
        {
            AnsiConsole.WriteLine("No users found.");
            Log("GetAllUsers called, but no users found.");
            return null;
        }
        Log($"GetAllUsers called and found ${users.Count} users.");
        return users;
    }
}