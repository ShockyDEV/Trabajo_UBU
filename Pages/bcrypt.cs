using System;
using System.Security.Cryptography;
using System.Text;

public static class Bcrypt
{
    private const int SaltSize = 16;
    private const int WorkFactor = 100000; 

    public static string HashPassword(string password)
    {
        var salt = GenerateSalt();
        return HashPassword(password, salt);
    }

    private static string HashPassword(string password, byte[] salt)
    {
        using (var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, salt, WorkFactor, HashAlgorithmName.SHA256))
        {
            var hash = rfc2898DeriveBytes.GetBytes(32); // SHA-256 hash
            var hashBytes = new byte[SaltSize + hash.Length];
            Buffer.BlockCopy(salt, 0, hashBytes, 0, SaltSize);
            Buffer.BlockCopy(hash, 0, hashBytes, SaltSize, hash.Length);
            return Convert.ToBase64String(hashBytes);
        }
    }

    public static bool VerifyPassword(string password, string hashedPassword)
    {
        var hashBytes = Convert.FromBase64String(hashedPassword);
        var salt = new byte[SaltSize];
        Buffer.BlockCopy(hashBytes, 0, salt, 0, SaltSize);
        var hash = HashPassword(password, salt);
        return hash == hashedPassword;
    }

    private static byte[] GenerateSalt()
    {
        var salt = new byte[SaltSize];
        RandomNumberGenerator.Fill(salt); 
        return salt;
    }
}
