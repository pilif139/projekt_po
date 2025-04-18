using System.Security.Cryptography;
using System.Text;

namespace projekt_po.Utils;

/// <summary>
/// Utility class for hashing and comparing hashed values.
/// </summary>
public class Hash
{
    /// <summary>
    /// Hashes a password using SHA-256.
    /// </summary>
    /// <param name="password">The password to hash.</param>
    /// <returns>The hashed password as a Base64 string.</returns>
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// Compares a word with a hashed value to check for equality.
    /// </summary>
    /// <param name="word">The plain text word to hash and compare.</param>
    /// <param name="hash">The hashed value to compare against.</param>
    /// <returns>True if the hashed word matches the provided hash; otherwise, false.</returns>
    public static bool CompareHash(string word, string hash)
    {
        string hashedWord = HashPassword(word);
        return hashedWord == hash;
    }
}
