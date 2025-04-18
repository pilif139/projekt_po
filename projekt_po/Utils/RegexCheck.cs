using System.Text.RegularExpressions;
    using Spectre.Console;
    
    namespace projekt_po.Utils;
    
    /// <summary>
    /// Provides regex-based validation methods for user input such as logins, passwords, and names.
    /// </summary>
    public partial class RegexCheck
    {
        /// <summary>
        /// Defines a regular expression for validating login names.
        /// Matches strings that are 3-20 characters long and contain only letters, numbers, underscores, and dashes.
        /// </summary>
        [GeneratedRegex(@"^[a-zA-Z0-9_-]{3,20}$")]
        private static partial Regex LoginRegex();
    
        /// <summary>
        /// Defines a regular expression that matches any letter character.
        /// </summary>
        [GeneratedRegex(@"[a-zA-Z]")]
        private static partial Regex LettersRegex();
    
        /// <summary>
        /// Defines a regular expression that matches any digit character.
        /// </summary>
        [GeneratedRegex(@"\d")]
        private static partial Regex NumbersRegex();
    
        /// <summary>
        /// Defines a regular expression that matches special characters commonly used in passwords.
        /// </summary>
        [GeneratedRegex(@"[@$!%*?&]")]
        private static partial Regex SpecialCharsRegex();
        
        /// <summary>
        /// Validates a login string against predefined rules.
        /// </summary>
        /// <param name="login">The login string to validate.</param>
        /// <returns>
        /// True if the login meets all validation requirements; otherwise, false.
        /// If validation fails, error messages are displayed to the console.
        /// </returns>
        /// <remarks>
        /// Validation rules:
        /// - Must be 3-20 characters long
        /// - Can only contain letters, numbers, underscores, and dashes
        /// - Cannot contain spaces
        /// - Cannot contain two consecutive dots
        /// </remarks>
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
    
        /// <summary>
        /// Validates a password string against predefined security rules.
        /// </summary>
        /// <param name="password">The password string to validate.</param>
        /// <returns>
        /// True if the password meets all security requirements; otherwise, false.
        /// If validation fails, error messages are displayed to the console.
        /// </returns>
        /// <remarks>
        /// Validation rules:
        /// - Must be at least 8 characters long
        /// - Must contain at least one letter
        /// - Must contain at least one special character (@$!%*?&)
        /// - Must contain at least one number
        /// - Cannot contain spaces
        /// </remarks>
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
    
        /// <summary>
        /// Validates a name or surname string against predefined rules.
        /// </summary>
        /// <param name="name">The name or surname string to validate.</param>
        /// <returns>
        /// True if the name meets all validation requirements; otherwise, false.
        /// If validation fails, error messages are displayed to the console.
        /// </returns>
        /// <remarks>
        /// Validation rules:
        /// - Must be 3-20 characters long
        /// - Must contain only letters (no numbers or special characters)
        /// - Cannot contain spaces
        /// </remarks>
        public static bool IsValidNameAndSurname(string name)
        {
            // regex check if name has 3-20 characters, only letters, numbers, underscores and dashes
            bool isValid = true;
            if (name.Length < 3)
            {
                AnsiConsole.MarkupLine("[red]Name and surname must be at least 3 characters long.[/]");
                isValid = false;
            }
            if (name.Length > 20)
            {
                AnsiConsole.MarkupLine("[red]Name and surname must be at most 20 characters long.[/]");
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