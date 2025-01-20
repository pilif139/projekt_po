namespace projekt_po.Utils;

public class StringUtils
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
}