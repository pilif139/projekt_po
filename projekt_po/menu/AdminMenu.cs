using projekt_po.Model;
using projekt_po.Services;
using Spectre.Console;

namespace projekt_po.Menu;

public class AdminMenu
{
    private readonly UserService _userService;
    public AdminMenu(UserService userService)
    {
        _userService = userService;
    }
    
    public void ShowAdminMenu() //function that shows admin menu
     {
         Console.Clear();
         while (true)
         {
             Console.Clear();
             var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                 .Title("[red]Admin menu[/]")
                 .PageSize(10)
                 .AddChoices(new[]
                 {
                     "Add user",
                     "Delete user",
                     "List of users",
                     "Logout"
                 }));
             switch (option)
             {
                 case "Add user":
                     AddUser();
                     break;
                 case "Delete user":
                     DeleteUser();
                     break;
                 case "List of users":
                     ListUsers();
                     break;
                 case "Logout":
                     Console.WriteLine("Logging out...");
                     return;
             }
         }
     }

    private void AddUser()
    {
        Console.Clear();
        AnsiConsole.MarkupLine("[red]Creating new user[/]");
        string name = AnsiConsole.Prompt(new TextPrompt<string>("Enter username: "));
        string surname = AnsiConsole.Prompt(new TextPrompt<string>("Enter surname: "));
        string password = AnsiConsole.Prompt(new TextPrompt<string>("Enter password: "));
        var role = AnsiConsole.Prompt(
            new SelectionPrompt<Role>()
                .Title("Choose role")
                .PageSize(3)
                .AddChoices(new[]
                {
                    Role.Admin,
                    Role.User
                }));
        _userService.AddUser(name, surname, password, role);
    }
    
    private void DeleteUser()
    {
        Console.Clear();
        var users = _userService.GetAllUsers();
        if (users == null)
        {
            Console.WriteLine("No users to delete found.");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }
        var usersToDelete = AnsiConsole.Prompt(
            new MultiSelectionPrompt<User>()
                .Title("Choose users to delete")
                .NotRequired()
                .PageSize(15)
                .MoreChoicesText("[grey](Move up and down))[/]")
                .InstructionsText(
                    "[grey](Press [blue]<space>[/] to pick a user to delete, " + 
                    "[green]<enter>[/] to accept)[/]")
                .AddChoices(users));
        
        foreach (var user in usersToDelete)
        {
            _userService.DeleteUser(user.Id);
        }
        if(usersToDelete.Count > 0)
        {
            Console.WriteLine("Users deleted successfully.");
        }
        else
        {
            Console.WriteLine("No users deleted.");
        }

        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
    
    private void ListUsers()
    {
        Console.Clear();
        var users = _userService.GetAllUsers();
        AnsiConsole.Markup("[red]List of users[/]\n");
        if (users == null)
        {
            Console.WriteLine("No users found.");
        }
        else
        {
            foreach (var user in users)
            {
                Console.WriteLine($"Id: {user.Id}, Name: {user.Name}, Surname: {user.Surname}, Role: {user.Role}");
            }
        }
        
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}