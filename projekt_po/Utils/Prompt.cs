using Spectre.Console;

namespace projekt_po.Utils;

/// <summary>
/// Utility class for prompting user input using Spectre.Console.
/// </summary>
public static class Prompt
{
    /// <summary>
    /// Prompts the user to enter a string with optional validation functions.
    /// </summary>
    /// <param name="prompt">The prompt message to display.</param>
    /// <param name="regexFuncs">Optional validation functions for the input.</param>
    /// <returns>The user input as a string.</returns>
    public static string GetString(string prompt, params Func<string, bool>[] regexFuncs)
    {
        return GetString(prompt, false, regexFuncs);
    }
    
    /// <summary>
    /// Prompts the user to enter a string with optional validation functions and secret input mode.
    /// </summary>
    /// <param name="prompt">The prompt message to display.</param>
    /// <param name="isSecret">Whether the input should be hidden (e.g., for passwords).</param>
    /// <param name="regexFuncs">Optional validation functions for the input.</param>
    /// <returns>The user input as a string.</returns>
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

    /// <summary>
    /// Prompts the user to enter a number within a specified range.
    /// </summary>
    /// <typeparam name="T">The numeric type (e.g., int, double).</typeparam>
    /// <param name="prompt">The prompt message to display.</param>
    /// <param name="min">The minimum allowed value.</param>
    /// <param name="max">The maximum allowed value.</param>
    /// <returns>The user input as a number of type T.</returns>
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
                    return ValidationResult.Success();
                }
            );
        return AnsiConsole.Prompt(numberPrompt);
    }

    /// <summary>
    /// Prompts the user to select a single item from a list.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    /// <param name="prompt">The prompt message to display.</param>
    /// <param name="list">The list of items to choose from.</param>
    /// <returns>The selected item.</returns>
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
    
    /// <summary>
    /// Prompts the user to select a single value from an enumeration.
    /// </summary>
    /// <typeparam name="TEnum">The enumeration type.</typeparam>
    /// <param name="prompt">The prompt message to display.</param>
    /// <param name="items">Optional enumeration values to choose from.</param>
    /// <returns>The selected enumeration value.</returns>
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
    
    /// <summary>
    /// Prompts the user to select multiple items from a list.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    /// <param name="prompt">The prompt message to display.</param>
    /// <param name="list">The list of items to choose from.</param>
    /// <returns>A list of selected items.</returns>
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
    
    /// <summary>
    /// Prompts the user to enter a date with optional validation.
    /// </summary>
    /// <param name="prompt">The prompt message to display.</param>
    /// <param name="validate">Optional validation function for the date.</param>
    /// <returns>The user input as a DateTime object.</returns>
    public static DateTime GetDate(string prompt, Func<DateTime, bool>? validate = null)
    {
        const string format = "yyyy-MM-dd HH:mm";
        var input = AnsiConsole.Prompt(
            new TextPrompt<DateTime>(prompt.Length > 0 ? $"{prompt} {format}" : $"Enter a date {format}:")
                .InvalidChoiceMessage("Invalid date")
                .Validate((val) =>
                {
                    if (validate != null && !validate(val))
                    {
                        return ValidationResult.Error("Invalid date");
                    }
                    return ValidationResult.Success();
                })
                .PromptStyle("grey"));
        return input;
    }
}
