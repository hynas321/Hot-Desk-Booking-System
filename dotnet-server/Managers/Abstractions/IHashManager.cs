namespace Dotnet.Server.Managers;

public interface IHashManager
{
    string HashPassword(string password);
    bool VerifyPassword(string userInputPassword, string storedHashedPassword);
}
