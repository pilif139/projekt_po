 using Microsoft.EntityFrameworkCore.Diagnostics;
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

     public void ShowMenu()
     {
         var menuOption = AnsiConsole.Prompt(
             new SelectionPrompt<string>()
                 .Title("[red]Bowling alley reservation system[/]?")
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

     private void Login()
     {
         string name = AnsiConsole.Prompt(new TextPrompt<string>("[red]Username[/]?"));
         string surname = AnsiConsole.Prompt(new TextPrompt<string>("[red]Surname[/]?"));
         string password = AnsiConsole.Prompt(new TextPrompt<string>("[red]Surname[/]?"));
         bool isAuthenticated=_authService.Authenticate(name,surname,password);
         if (isAuthenticated)
         {
             AnsiConsole.MarkupLine("[green]Successful login[/]");
             AnsiConsole.MarkupLine($"[green]Welcome {name}[/]");
         }
         else
         {
             AnsiConsole.MarkupLine("[red]Unsuccessful login[/]");
         }
     }
 }