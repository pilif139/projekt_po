using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Services;

public interface IAuthService
{
    bool Authenticate(string login, string password);
    bool Register(string login,string name, string surname, string password, Role role);
    User? GetLoggedUser();
    Role GetLoggedUserRole();
    bool IsUserLogged();
    void Logout();
}

public class AuthService : BaseService, IAuthService
{
    private User? _loggedUser;
    private readonly IUserRepository _userRepository;

    public AuthService(IUserRepository userRepository, ILogger logger) : base(logger)
    {
        _userRepository = userRepository;
    }

    public bool Authenticate(string login,string password)
    {
        // first checks if the user exists in the database
        var user = _userRepository.GetByLogin(login);
        if (user == null)
        {
            AnsiConsole.MarkupLine("User not found.");
            Log($"Tried to login with non-existent user {login}.");
            return false;
        }
        // compare hashed password with the hased one in the database
        if (Hash.CompareHash(password, user.Password))
        {
            AnsiConsole.MarkupLine("Login successful.");
            Log($"User {login} logged in.");
            _loggedUser = user;
            return true;
        }
        // else return false 
        Log($"User {login} tried to login with wrong password.");
        return false;
    }

    public bool Register(string login,string name, string surname, string password, Role role)
    {
        // checks if user with the same name and surname already exists
        if (!RegexCheck.IsValidLogin(login) || !RegexCheck.IsValidPassword(password) || !RegexCheck.IsValidNameAndSurname(name) || !RegexCheck.IsValidNameAndSurname(surname))
        {
            Log("Tried to register with invalid data.");
            return false;
        }
        string hashedPassword = Hash.HashPassword(password);    
        // insert user into the database and set user context to the logged in user
        var user = _userRepository.Add(login, name, surname, hashedPassword, role);
        AnsiConsole.MarkupLine("User added successfully.");
        Log($"User {name} {surname} with role {role} added.");
        _loggedUser = user;
        return true;
    }

    public User? GetLoggedUser()
    {
        if (_loggedUser == null)
        {
            Log("Tried to get not logged in user.");
            return null;
        }
        Log($"User {_loggedUser.Name} {_loggedUser.Surname} requested.");
        return _loggedUser;
    }

    public Role GetLoggedUserRole()
    {
        if (_loggedUser == null)
        {
            Log("Tried to get role of not logged in user.");
            return Role.None;
        }
        return _loggedUser.Role;
    }

    public bool IsUserLogged()
    {
        return _loggedUser != null;
    }

    public void Logout()
    {
        // if someone tries to logout without being logged in
        if (_loggedUser == null)
        {
            Console.WriteLine("No user logged in.");
            Log("Tried to logout without being logged in.");
            return;
        }
        Console.WriteLine($"User {_loggedUser.Name} {_loggedUser.Surname} logged out.");
        Log($"User {_loggedUser.Name} {_loggedUser.Surname} logged out.");
        _loggedUser = null;
    }

}