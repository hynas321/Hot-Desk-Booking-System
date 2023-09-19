using System.Security.Cryptography;

namespace Dotnet.Server.Managers;

public class HashManager
{
    public string HashPassword(string password)
    {
        byte[] salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using (var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256))
        {
            byte[] hash = pbkdf2.GetBytes(32);
            byte[] combinedBytes = new byte[48];
            Array.Copy(salt, 0, combinedBytes, 0, 16);
            Array.Copy(hash, 0, combinedBytes, 16, 32);

            return Convert.ToBase64String(combinedBytes);
        }
    }

    public bool VerifyPassword(string userInputPassword, string storedHashedPassword)
    {
        byte[] combinedBytes = Convert.FromBase64String(storedHashedPassword);
        byte[] salt = new byte[16];
        byte[] storedHash = new byte[32];

        Array.Copy(combinedBytes, 0, salt, 0, 16);
        Array.Copy(combinedBytes, 16, storedHash, 0, 32);

        using (var pbkdf2 = new Rfc2898DeriveBytes(userInputPassword, salt, 10000, HashAlgorithmName.SHA256))
        {
            byte[] userHash = pbkdf2.GetBytes(32);

            for (int i = 0; i < 32; i++)
            {
                if (userHash[i] != storedHash[i])
                {
                    return false;
                }
            }
        }

        return true;
    }
}