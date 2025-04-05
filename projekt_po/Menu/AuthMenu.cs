using projekt_po.Model;
using projekt_po.Services;
using projekt_po.Utils;
using Spectre.Console;

namespace projekt_po.Menu;

public class AuthMenu
{
    private readonly IAuthService _authService;

    public AuthMenu(IAuthService authService)
    {
        _authService = authService;
    }

    public bool Show()
    {
        Console.Clear();
        var menuOption = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("[red]Bowling alley reservation system[/]")
                .PageSize(10)
                .AddChoices(new[]
                {
                    "Login",
                    "Register as client",
                    "Exit"
                }));
        switch (menuOption)
        {
            case "Login":
                // if the login is succesful it will return true and the main loop in Program.cs will continue
                return Login();
            case "Register as client":
                return Register();
            case "Exit":
                var text = new FigletText("Thanks for using our system!").Centered().Color(Color.Green);
                    
                AnsiConsole.Write(text);
                // returns false to tell the loop to stop
                return false;
            default:
                return false;
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
            AnsiConsole.MarkupLine($"[rapidblink {triesLeftColor[triesLeft]}]Tries left: {triesLeft}[/]");
            
            string login = Prompt.GetString("Enter your login:");
            string password = Prompt.GetString("Enter your password", isSecret: true);
            bool isAuthenticated = _authService.Authenticate(login, password);
            if (isAuthenticated)
            {
                var name = _authService.GetLoggedUser()!.Name;
                AnsiConsole.MarkupLine("[green]Successful login[/]");
                AnsiConsole.MarkupLine($"[green]Welcome {name}[/]");
                return true;
            }
            AnsiConsole.MarkupLine("[red]Unsuccessful login[/]");
            Task.Delay(2000).Wait();
            triesLeft--;
        }

        return false;
    }

    public bool Register()
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.MarkupLine("[blue]Register new client account[/]");
            string login = Prompt.GetString("Enter your login:", RegexCheck.IsValidLogin);
            string name = Prompt.GetString("Enter your name:", RegexCheck.IsValidNameAndSurname);
            string surname = Prompt.GetString("Enter your surname:", RegexCheck.IsValidNameAndSurname);
            string password = Prompt.GetString("Enter your password", RegexCheck.IsValidPassword, isSecret: true);
            string passwordRepeat = Prompt.GetString("Repeat your password", isSecret: true);
            if (password != passwordRepeat)
            {
                AnsiConsole.MarkupLine("[red]Passwords do not match[/]");
                Task.Delay(2000).Wait();
                continue;
            }
            if (_authService.Register(login, name, surname, password, Role.Client))
            {
                AnsiConsole.MarkupLine("[green]Account created successfully[/]");
                Task.Delay(2000).Wait();
                return true;
            }
            AnsiConsole.MarkupLine("[blue]Press any key to try again or backspace to go back to main menu.[/]");
            char key = Console.ReadKey(true).KeyChar;
            if (key == '\u0008') // backspace key
            {
                return false;
            }
        }
    }
}