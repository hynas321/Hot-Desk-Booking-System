namespace Dotnet.Server.Managers;

public class SessionTokenManager
{
    private static Dictionary<string, string> tokens = new Dictionary<string, string>();

    public string CreateToken(string username)
    {
        string token = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 16);
        tokens.Add(token, username);

        return token;
    }

    public bool RemoveToken(string token)
    {
        return tokens.Remove(token);
    }

    public string? GetUsername(string token)
    {
        if (tokens.TryGetValue(token, out string? username))
        {
            return username;
        }
        else
        {
            return null; 
        }
    }

    public Dictionary<string, string> GetAllSessions()
    {
        return tokens;
    }
}