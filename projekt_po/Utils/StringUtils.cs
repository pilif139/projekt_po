using System.Text.RegularExpressions;

namespace projekt_po.Utils;

public static class StringUtils
{
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
    
    public static bool ContainsSpecialCharacters(string str)
    {
        return Regex.IsMatch(str, @"[!@#$%^&*()_+=\[{\]};:<>|./?,-]");
    }
}