using projekt_po.Services;
using Spectre.Console;

namespace projekt_po.Menu;

/// <summary>
/// base class for all menu except auth menu that controls if the program is running. 
/// In the constructor it takes the title of the menu and the auth service that will be used to log out
/// You can add menu options by calling AddMenuOption and passing the name of the option and the action/method that will be executed
/// It already add option to logout.
/// </summary>
public abstract class BaseMenu
{
    private readonly Dictionary<string, Action> _menuOptions;
    private string Title { get; set; }
    private readonly IAuthService _authService;
    
    protected BaseMenu(string title, IAuthService authService)
    {
        Title = title;
        _menuOptions = new Dictionary<string, Action>();
        _authService = authService;
    }
    
    protected void AddMenuOption(string option, Action action)
    {
        _menuOptions.TryAdd(option, action);
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            var option = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .Title($"[blue]{Title}[/]")
                .PageSize(10)
                .AddChoices(_menuOptions.Keys.Concat(["Logout"]).ToArray()));
            if(option == "Logout")
            {
                _authService.Logout();
                return;
            }
            foreach(var menuOption in _menuOptions)
            {
                if (menuOption.Key == option)
                {
                    menuOption.Value.Invoke();
                    break;
                }
            }
        }
    }
}