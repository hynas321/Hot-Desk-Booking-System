namespace WebApi.Managers.Abstractions;

public interface ISessionTokenManager
{
    string CreateToken(string username, string role);
    string? RefreshToken(string token);
}
