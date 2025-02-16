 using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
 using Microsoft.EntityFrameworkCore.Diagnostics;
 using projekt_po.Model;
 using projekt_po.Services;
 using Spectre.Console;

 public class UserMenu
 {
     private readonly UserService _userService;
     private readonly AuthService _authService;

     public UserMenu(UserService userService,AuthService authService)
     {
         _userService = userService;
         _authService = authService;
     }

     public void AdminRoleMenu() //function that shows admin menu
     {
         Console.Clear();
         bool Menu = true;
         while (Menu)
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
                     Console.Clear();
                     AnsiConsole.MarkupLine("[red]Creating new user[/]");
                     string name = AnsiConsole.Prompt(new TextPrompt<string>("Enter username: "));
                     string surname = AnsiConsole.Prompt(new TextPrompt<string>("Enter surname: "));
                     string password = AnsiConsole.Prompt(new TextPrompt<string>("Enter password: "));
                     string role = AnsiConsole.Prompt(new TextPrompt<string>("Enter role(admin or user): "));
                     if (role.ToLower() == "admin")
                     {
                         _userService.AddUser(name, surname, password, Role.Admin);
                     }
                     else if (role.ToLower() == "user")
                     {
                         _userService.AddUser(name, surname, password, Role.User);
                     }


                     break;
                 case "Delete user":
                     Console.Clear();
                     _userService.ListUsers();
                     int id = AnsiConsole.Prompt(new TextPrompt<int>("Enter user id: "));
                     _userService.DeleteUser(id);
                     Console.ReadKey();
                     break;
                 case "List of users":
                     Console.Clear();
                     _userService.ListUsers();
                     Console.ReadKey();

                     break;
                 case "Logout":
                     Console.WriteLine("Logging out...");
                     Menu = false;
                     break;
             }
         }

         ShowMenu();
     }

     public void UserRoleMenu()
     {
         Console.Clear();
         bool Menu = true;
         while (Menu)
         {
             Console.Clear();
             var Option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                 .Title("[red]User menu[/]")
                 .PageSize(10)
                 .AddChoices(new[]
                 {
                     "Make reservation",
                     "Your reservations",
                     "Logout"
                 }));
             switch (Option)
             {
                 case "Make reservation":
                     Console.Clear();
                     Console.WriteLine("BEDZIE ROBIENIE REZERWACJI");
                     break;
                 case "Your reservations":
                     Console.Clear();
                     Console.WriteLine("BEDA TUTAJ REZERWACJE");
                     break;
                 case "Logout":
                     Console.WriteLine("Logging out...");
                     Menu = false;
                     break;
             }
         }
         ShowMenu();
     }



     public void ShowMenu()
     {
         Console.Clear();
         var menuOption = AnsiConsole.Prompt(
             new SelectionPrompt<string>()
                 .Title("[red]Bowling alley reservation system[/]")
                 .PageSize(10)
                 .AddChoices(new[] {
                     "Login",
                     "Exit"
                 }));
         switch (menuOption)
         {
             case "Login":
                 Login();
                 
                
                 
                 break;
             case "Exit":
                 Console.WriteLine("Thanks for using our system");
                 break;
         }
     }

     public void Login()
     {
         bool tries = true;
         int triesLeft = 3;
         while (tries)
         {
             Console.Clear();
             AnsiConsole.MarkupLine("[red]Login to your account[/]");
             if (triesLeft==3)
             {
                 AnsiConsole.MarkupLine($"[green]Tries left: {triesLeft}[/]");
             }
             else if (triesLeft == 2)
             {
                 AnsiConsole.MarkupLine($"[yellow]Tries left: {triesLeft}[/]");
             }
             else if (triesLeft == 1)
             {
                 AnsiConsole.MarkupLine($"[red]Tries left: {triesLeft}[/]");
             }
             
             string name = AnsiConsole.Prompt(new TextPrompt<string>("Enter your username:"));
             string surname = AnsiConsole.Prompt(new TextPrompt<string>("Enter your surname:"));
             string password = AnsiConsole.Prompt(new TextPrompt<string>("Enter your password:"));
             bool isAuthenticated = _authService.Authenticate(name, surname, password);
             if (isAuthenticated)
             {
                 AnsiConsole.MarkupLine("[green]Successful login[/]");
                 var role = _authService.GetLoggedUserRole();
                 AnsiConsole.MarkupLine($"[green]Welcome {name}[/]");
                 
                 if (role == Role.Admin)
                 {
                     AdminRoleMenu();
                 }
                 else if (role == Role.User)
                 {
                     UserRoleMenu();
                 }
                 tries = false;
             }
             else
             {
                 AnsiConsole.MarkupLine("[red]Unsuccessful login[/]");
                 triesLeft--;
                 if (triesLeft == 0)
                 {
                     tries = false;
                     ShowMenu();
                 }
             }
         }
         
     }
 }