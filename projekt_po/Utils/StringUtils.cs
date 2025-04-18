using System.Text.RegularExpressions;

namespace projekt_po.Utils;

public static class StringUtils
{
    /// <summary>
    /// Checks if any of the provided strings are null or empty.
    /// </summary>
    /// <param name="strings">An array of strings to check.</param>
    /// <returns>True if any string is null or empty; otherwise, false.</returns>
    public static bool AreStringsNullOrEmpty(params string?[] strings)
    {
        foreach (var str in strings)
        {
            if (string.IsNullOrEmpty(str))
            {
                return true;
            }
        }

        return false;
    }
    
    /// <summary>
    /// Determines if the given string contains any special characters.
    /// </summary>
    /// <param name="str">The string to check for special characters.</param>
    /// <returns>True if the string contains special characters; otherwise, false.</returns>
    public static bool ContainsSpecialCharacters(string str)
    {
        return Regex.IsMatch(str, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
    }
}
