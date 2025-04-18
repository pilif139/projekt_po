using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Services;

/// <summary>
/// Service for managing users in the system.
/// </summary>
public class UserService : BaseService, IModelService<User>
{
    private const Resource Resource = Services.Resource.User;
    private readonly IUserRepository _userRepository;
    private readonly IRbacService _rbacService;
    public UserService(IUserRepository userRepository, IRbacService rbacService, ILogger logger) : base(logger)
    {
        _userRepository = userRepository;
        _rbacService = rbacService;
    }


    public bool Add(User user)
    {
        if (!_rbacService.CheckPermission(Resource, Permission.Create)) return false;
        //chceck if user with this login already exists
        if (_userRepository.GetByLogin(user.Login) != null)
        {
            AnsiConsole.MarkupLine("User with this login already exists.");
            Log($"Tried to add user with existing login {user.Login}.");
            return false;
        }
        // validation of user fields
        if(!RegexCheck.IsValidLogin(user.Login) || !RegexCheck.IsValidPassword(user.Password) || !RegexCheck.IsValidNameAndSurname(user.Name) || !RegexCheck.IsValidNameAndSurname(user.Surname))
        {
            AnsiConsole.MarkupLine("Invalid user data.");
            Log($"Tried to add user with invalid data: {user.Login}, {user.Name}, {user.Surname}.");
            return false;
        }
        string hashedPassword = Hash.HashPassword(user.Password);
        _userRepository.Add(user.Login,user.Name, user.Surname, hashedPassword, user.Role);
        AnsiConsole.MarkupLine("User added successfully.");
        Log($"User {user.Name} {user.Surname} with role {user.Role} added.");
        return true;
    }

    public bool Delete(int id)
    {
        if (!_rbacService.CheckPermission(Resource, Permission.Delete)) return false;
        bool success = _userRepository.Delete(id);
        if (success)
        {
            AnsiConsole.MarkupLine($"User with id {id} deleted successfully.");
            Log($"User with id {id} deleted.");
        }
        else
        {
            AnsiConsole.MarkupLine("User not found.");
            Log($"Tried to delete non-existent user with {id} id.");
        }

        return success;
    }

    public bool Update(User user)
    {
        var existingUser = _userRepository.GetById(user.Id);
        if (existingUser == null)
        {
            AnsiConsole.MarkupLine("User not found.");
            Log($"Tried to update non-existent user with {user.Id} id.");
            return false;
        }
        if (!_rbacService.CheckPermission(Resource, Permission.Update, existingUser)) return false;
        string hashedPassword = Hash.HashPassword(user.Password);
        user.Password = hashedPassword;

        bool success = _userRepository.Update(user);
        if (success)
        {
            AnsiConsole.MarkupLine($"User with id {user.Id} updated successfully.");
            Log($"User with id {user.Id} updated.");
        }
        else
        {
            AnsiConsole.MarkupLine("User not found.");
            Log($"Tried to update non-existent user with {user.Id} id.");
        }
        return success;
    }

    public User? GetById(int id)
    {
        if (!_rbacService.CheckPermission(Resource, Permission.Read)) return null;
        var user = _userRepository.GetById(id);
        if (user == null)
        {
            AnsiConsole.MarkupLine("User not found");
            Log($"User with id {id} not found.");
            return null;
        }
        Log($"User with id {id} found.");
        return user;
    }

    public List<User>? GetAll()
    {
        if (!_rbacService.CheckPermission(Resource, Permission.Read)) return null;
        var users = _userRepository.GetAll();
        if (users.Count == 0)
        {
            AnsiConsole.MarkupLine("No users found.");
            Log("GetAllUsers called, but no users found.");
            return null;
        }
        Log($"GetAllUsers called and found ${users.Count} users.");
        return users;
    }

    public List<User>? GetAllByRole(params List<Role> roles)
    {
        if (!_rbacService.CheckPermission(Resource, Permission.Read)) return null;
        List<User> users = new List<User>();
        foreach (var role in roles)
        {
            users.AddRange(_userRepository.GetAllByRole(role));
        }
        if (users.Count == 0)
        {
            AnsiConsole.MarkupLine("No users found.");
            Log("GetAllUsers called, but no users found.");
            return null;
        }
        Log($"GetAllUsers called and found ${users.Count} users.");
        return users;
    }
    
    public User? GetByLogin(string login)
    {
        if (!_rbacService.CheckPermission(Resource, Permission.Read)) return null;
        var user = _userRepository.GetByLogin(login);
        if (user == null)
        {
            Log($"User with login {login} not found.");
            return null;
        }
        Log($"User with login {login} found.");
        return user;
    }
}