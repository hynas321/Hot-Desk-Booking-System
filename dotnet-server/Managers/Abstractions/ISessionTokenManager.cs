namespace Dotnet.Server.Managers;

public interface ISessionTokenManager
{
    string CreateToken(string username);
    bool RemoveToken(string token);
    string? GetUsername(string token);
    Dictionary<string, string> GetAllSessions();
}
