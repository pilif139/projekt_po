using projekt_po.Services;
using Spectre.Console;

namespace projekt_po.Menu;

public class AuthMenu
{
    private readonly IAuthService _authService;

    public AuthMenu(IAuthService authService)
    {
        _authService = authService;
    }

    public bool ShowMenu()
    {
        Console.Clear();
        var menuOption = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[red]Bowling alley reservation system[/]")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Login",
                    "Exit"
                }));
        switch (menuOption)
        {
            case "Login":
                // if the login is succesful it will return true and the main loop in Program.cs will continue
                return Login();
            case "Exit":
                Console.WriteLine("Thanks for using our system");
                // returns false to tell the loop to stop
                return false;
            default:
                return true;
        }
    }

    public bool Login()
    {
        int triesLeft = 3;
        while (triesLeft > 0)
        {
            Console.Clear();
            AnsiConsole.MarkupLine("[red]Login to your account[/]");
            var triesLeftColor = new Dictionary<int, string>()
            {
                { 3, "green" },
                { 2, "yellow" },
                { 1, "red" }
            };
            AnsiConsole.MarkupLine($"[{triesLeftColor[triesLeft]}]Tries left: {triesLeft}[/]");

            string name = AnsiConsole.Prompt(new TextPrompt<string>("Enter your username:"));
            string surname = AnsiConsole.Prompt(new TextPrompt<string>("Enter your surname:"));
            string password = AnsiConsole.Prompt(new TextPrompt<string>("Enter your password:").Secret()); // Secret() is for password inputs
            bool isAuthenticated = _authService.Authenticate(name, surname, password);
            if (isAuthenticated)
            {
                AnsiConsole.MarkupLine("[green]Successful login[/]");
                AnsiConsole.MarkupLine($"[green]Welcome {name}[/]");
                return true;
            }
            AnsiConsole.MarkupLine("[red]Unsuccessful login[/]");
            triesLeft--;
        }

        return false;
    }
}