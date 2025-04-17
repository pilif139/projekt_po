using Spectre.Console;

namespace projekt_po.Utils;

public static class Prompt
{
    public static string GetString(string prompt, params Func<string, bool>[] regexFuncs)
    {
        return GetString(prompt, false, regexFuncs);
    }
    
    public static string GetString(string prompt,bool isSecret = false, params Func<string, bool>[] regexFuncs)
    {
        var textPrompt =
            new TextPrompt<string>(prompt.Length > 0 ? $"{prompt} " : "Enter a string:")
                .InvalidChoiceMessage("String cannot be empty")
                .PromptStyle("grey")
                .Validate((val) =>
                    {
                        if (regexFuncs == null || regexFuncs.Length == 0)
                        {
                            return ValidationResult.Success();
                        }
                        foreach(var func in regexFuncs)
                        {
                            if (!func(val))
                            {
                                return ValidationResult.Error("Invalid input.");
                            }
                        }
                        return ValidationResult.Success();
                    }
                );

        if (isSecret)
        {
            textPrompt = textPrompt.Secret();
        }
        return AnsiConsole.Prompt(textPrompt);
    }

    public static T GetNumber<T>(string prompt, T min, T max)
        where T : struct, IComparable<T>
    {
        var numberPrompt = new TextPrompt<T>(prompt.Length > 0 ? $"{prompt} " : $"Enter a {typeof(T)} number: ")
            .InvalidChoiceMessage($"Number must be between {min} and {max}")
            .PromptStyle("grey")
            .Validate((val) =>
                {
                    if (val.CompareTo(min) < 0)
                    {
                        return ValidationResult.Error($"Number must be greater than {min}");
                    }
                    if (val.CompareTo(max) > 0)
                    {
                        return ValidationResult.Error($"Number must be less than {max}");
                    }
                    return ValidationResult.Error("Invalid input.");
                }
            );
        return AnsiConsole.Prompt(numberPrompt);
    }

    public static T SelectFromList<T>(string prompt, List<T> list) where T : class
    {
        if (list.Count == 0)
        {
            throw new ArgumentException("List cannot be empty");
        }
        var selected = AnsiConsole.Prompt(
            new SelectionPrompt<T>()
                .Title(prompt)
                .PageSize(15)
                .MoreChoicesText("[grey](Move up and down)[/]")
                .AddChoices(list)
            );
        return selected;
    }
    
    // implementation of SelectFromList for enums
    public static TEnum SelectFromList<TEnum>(string prompt, IEnumerable<TEnum> items = null) where TEnum : struct, Enum
    {
        items ??= Enum.GetValues(typeof(TEnum)).Cast<TEnum>();
        return AnsiConsole.Prompt(
            new SelectionPrompt<TEnum>()
                .Title(prompt)
                .PageSize(15)
                .MoreChoicesText("[grey](Move up and down)[/]")
                .AddChoices(items)
            );
    }
    
    public static List<T> SelectMultipleFromList<T>(string prompt, List<T> list) where T : class
    {
        if (list.Count == 0)
        {
            throw new ArgumentException("List cannot be empty");
        }
        var selected = AnsiConsole.Prompt(
                new MultiSelectionPrompt<T>()
                    .Title(prompt)
                    .NotRequired()
                    .PageSize(15)
                    .MoreChoicesText("[grey](Move up and down))[/]")
                    .InstructionsText(
                        "[grey](Press [blue]<space>[/] to pick a user to delete, " +
                        "[green]<enter>[/] to accept)[/]")
                    .AddChoices(list));
        
        return selected;
    }
    
    public static DateTime GetDate(string prompt)
    {
        const string format = "yyyy-MM-dd HH:mm";
        var input = AnsiConsole.Prompt(
            new TextPrompt<DateTime>(prompt.Length > 0 ? $"{prompt} {format}" : $"Enter a date {format}:")
                .InvalidChoiceMessage("Invalid date")
                .PromptStyle("grey"));
        return input;
    }
}