using projekt_po.Model;
using projekt_po.Repository;
using projekt_po.Utils;

namespace projekt_po.Services;

public class AuthService : BaseService
{
    private User? _loggedUser;
    private readonly IUserRepository _userRepository;
    
    public AuthService(IUserRepository userRepository, ILogger logger) : base(logger)
    {
        _userRepository = userRepository;
    }

    public bool Authenticate(string name, string surname, string password)
    {
        var user = _userRepository.GetByNameAndSurname(name, surname);
        if (user == null)
        {
            Console.WriteLine("User not found.");
            Log($"Tried to login with non-existent user {name} {surname}.");
            return false;
        }
        if (Hash.CompareHash(password, user.Password))
        {
            Console.WriteLine("Login successful.");
            Log($"User {name} {surname} logged in.");
            _loggedUser = user;
            return true;
        }
        Console.WriteLine("Wrong password.");
        Log($"User {name} {surname} tried to login with wrong password.");
        return false;
    }
    
    public void Register(string name, string surname, string password, Role role)
    {
        bool hasErrors = false;
        string hashedPassword = Hash.HashPassword(password);
        if(_userRepository.GetByNameAndSurname(name, surname) != null)
        {
            Console.WriteLine("User already exists.");
            Log($"Tried to register user {name} {surname} that already exists.");
            return;
        }
        if(name.Length < 3 || surname.Length < 3 || password.Length < 8)
        {
            Console.WriteLine("Name and surname must be at least 3 characters long, password must be at least 8 characters long.");
            Log($"Tried to register user {name} {surname} with too short name, surname or password.");
            hasErrors = true;
        }

        if (!StringUtils.ContainsSpecialCharacters(password))
        {
            Console.WriteLine("Password must contain at least one special character.");
            Log($"Tried to register user {name} {surname} with password that doesn't contain special character.");
            hasErrors = true;
        }

        if (hasErrors) return;
        
        var user = _userRepository.Add(name, surname, hashedPassword, role);
        Console.WriteLine("User added successfully.");
        Log($"User {name} {surname} with role {role} added.");
        _loggedUser = user;
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