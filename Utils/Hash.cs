﻿using System.Security.Cryptography;
using System.Text;

namespace projekt_po.Utils;

public class Hash
{
    public static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public static bool CompareHash(string word, string hash)
    {
        string hashedWord = HashPassword(word);
        return hashedWord == hash;
    }
}