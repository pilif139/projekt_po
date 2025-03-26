using Spectre.Console;

namespace projekt_po.Utils;

public class Prompt
{
    public static string GetString(string prompt)
    {
        var input = AnsiConsole.Prompt(
            new TextPrompt<string>(prompt.Length > 0 ? $"{prompt} " : "Enter a string:")
                .InvalidChoiceMessage("String cannot be empty")
                .PromptStyle("grey"));
        return input;
    }
    
    public static DateTime GetDate(string prompt)
    {
        var input = AnsiConsole.Prompt(
            new TextPrompt<DateTime>(prompt.Length > 0 ? $"{prompt} " : "Enter a date:")
                .InvalidChoiceMessage("Invalid date")
                .PromptStyle("grey"));
        return input;
    }
}