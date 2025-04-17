using projekt_po.Services;
using projekt_po.Utils;
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

    protected void ListItems<T>(List<T>? items) where T : IModelType
    {
        AnsiConsole.Clear();
        string modelName = typeof(T).Name;
        AnsiConsole.Markup($"[blue]List of {modelName}s[/]\n");
        if (items == null)
        {
            AnsiConsole.MarkupLine($"[red]No {modelName}s found.[/]");
        }
        else
        {
            var properties = typeof(T).GetProperties();

            var table = new Table().Centered();
            AnsiConsole.Live(table)
                .AutoClear(false)
                .Overflow(VerticalOverflow.Ellipsis)
                .Cropping(VerticalOverflowCropping.Bottom)
                .Start(ctx =>
                {
                    // add columns to the table
                    foreach (var prop in properties)
                    {
                        // properties that are not collections etc
                        if (!prop.PropertyType.IsGenericType &&
                            !typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) ||
                            prop.PropertyType == typeof(string))
                        {
                            table.AddColumn($"[bold cyan1]{prop.Name}[/]");
                            ctx.Refresh();
                            Task.Delay(100).Wait();
                        }
                    }
                    // add rows to the table
                    foreach (var item in items)
                    {
                        var rowData = new List<string>();
                        foreach (var prop in properties)
                        {
                            // same check as above
                            if (!prop.PropertyType.IsGenericType &&
                                !typeof(System.Collections.IEnumerable).IsAssignableFrom(prop.PropertyType) ||
                                prop.PropertyType == typeof(string))
                            {
                                var value = prop.GetValue(item);
                                rowData.Add(value?.ToString() ?? "");
                            }
                        }
                        table.AddRow(rowData.ToArray());
                        ctx.Refresh();
                        Task.Delay(100).Wait();
                    }

                    table.Border(TableBorder.Heavy);
                });
        }
        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
        Console.ReadKey();
    }
    
    protected void DeleteItems<T>(IModelService<T> service, List<T>? items) where T : class,IModelType
    {
        string modelName = typeof(T).Name;
        AnsiConsole.Clear();
        if (items == null || items.Count == 0)
        {
            AnsiConsole.MarkupLine("[red]Nothing to delete.[/]");
            AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
            Console.ReadKey();
            return;
        }

        var valuesToDelete = Prompt.SelectMultipleFromList($"Select {modelName}s to delete", items);

        foreach (var value in valuesToDelete)
        {
            service.Delete(value.Id);
        }

        if (valuesToDelete.Count > 0)
        {
            AnsiConsole.MarkupLine($"[green]{modelName}s deleted successfully.[/]");
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]No {modelName}s deleted.[/]");
        }

        AnsiConsole.MarkupLine("[yellow]Press any key to continue...[/]");
        Console.ReadKey();
    }
}