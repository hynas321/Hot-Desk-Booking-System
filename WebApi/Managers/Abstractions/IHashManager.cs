namespace WebApi.Managers.Abstractions;

public interface IHashManager
{
    string HashPassword(string password);
    bool VerifyPassword(string userInputPassword, string storedHashedPassword);
}
