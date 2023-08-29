namespace Dotnet.Server.Authentication;

public class SessionTokenManager
{
    private static Dictionary<string, string> tokens = new Dictionary<string, string>();

    public string CreateToken(string username)
    {
        string token = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 8);
        tokens.Add(token, username);

        return token;
    }

    public bool RemoveToken(string token)
    {
        return tokens.Remove(token);
    }

    public bool CheckIfUserLoggedIn(string token)
    {
        return tokens.Any(x => x.Key == token);
    }
}