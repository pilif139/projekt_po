using System.Text.RegularExpressions;
using Spectre.Console;

namespace projekt_po.Utils;

public partial class RegexCheck
{
    [GeneratedRegex(@"^[a-zA-Z0-9_-]{3,20}$")]
    private static partial Regex LoginRegex();
    
    [GeneratedRegex("@[a-zA-Z]")]
    private static partial Regex LettersRegex();
    
    [GeneratedRegex(@"\d")]
    private static partial Regex NumbersRegex();
    
    [GeneratedRegex(@"[@$!%*?&]")]
    private static partial Regex SpecialCharsRegex();
    
    public static bool IsValidLogin(string login)
    {
        // regex check if login has 3-20 characters, only letters, numbers, underscores and dashes
        bool isValid = true;
        if(!LoginRegex().IsMatch(login))
        {
            AnsiConsole.MarkupLine("[red]Login must be 3-20 characters long and can only contain letters, numbers, underscores and dashes.[/]");
            isValid = false;
        }
        if (login.Contains(' '))
        {
            AnsiConsole.MarkupLine("[red]Login cannot contain spaces.[/]");
            isValid = false;
        }
        if (login.Contains(".."))
        {
            AnsiConsole.MarkupLine("[red]Login cannot contain two dots in a row.[/]");
            isValid = false;
        }
        return isValid;
    }

    public static bool IsValidPassword(string password)
    {
        bool isValid = true;

        if (password.Length < 8)
        {
            AnsiConsole.MarkupLine("[red]Password must be at least 8 characters long.[/]");
            isValid = false;
        }

        if (!LettersRegex().IsMatch(password))
        {
            AnsiConsole.MarkupLine("[red]Password must contain at least one letter.[/]");
            isValid = false;
        }

        if (!SpecialCharsRegex().IsMatch(password))
        {
            AnsiConsole.MarkupLine("[red]Password must contain at least one special character.[/]");
            isValid = false;
        }
        
        if (!NumbersRegex().IsMatch(password))
        {
            AnsiConsole.MarkupLine("[red]Password must contain at least one number.[/]");
            isValid = false;
        }

        if (password.Contains(' '))
        {
            AnsiConsole.MarkupLine("[red]Password cannot contain spaces.[/]");
            isValid = false;
        }

        return isValid;
    }
    
    public static bool IsValidNameAndSurname(string name)
    {
        // regex check if name has 3-20 characters, only letters, numbers, underscores and dashes
        bool isValid = true;
        if (name.Length < 3)
        {
            AnsiConsole.MarkupLine("[red]Name and surname must be at least 3 characters long.[/]");
            isValid = false;
        }
        if (NumbersRegex().IsMatch(name) || SpecialCharsRegex().IsMatch(name))
        {
            AnsiConsole.MarkupLine("[red]Name and surname must contain only letters.[/]");
            isValid = false;
        }
        if (name.Contains(' '))
        {
            AnsiConsole.MarkupLine("[red]Name and surname cannot contain spaces.[/]");
            isValid = false;
        }
        return isValid;
    }
}