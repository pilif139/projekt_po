using Spectre.Console;

namespace projekt_po.Utils;

public static class Prompt
{
    public static string GetString(string prompt,Func<string, bool>? regexFunc = null ,bool isSecret = false)
    {
        var textPrompt =
            new TextPrompt<string>(prompt.Length > 0 ? $"{prompt} " : "Enter a string:")
                .InvalidChoiceMessage("String cannot be empty")
                .PromptStyle("grey")
                .Validate((val) =>
                    {
                        if (regexFunc == null || regexFunc(val))
                        {
                            return ValidationResult.Success();
                        }
                        return ValidationResult.Error("Invalid input.");
                    }
                );

        if (isSecret)
        {
            textPrompt = textPrompt.Secret();
        }
        return AnsiConsole.Prompt(textPrompt);
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