using Dotnet.Server;
using Dotnet.Server.Managers;

namespace Dotnet.Server.Tests;

public class UnitTests
{
    [Fact]
    public void CamelCaseToPascalCaseTest()
    {
        PascalCaseNamingPolicy policy = new PascalCaseNamingPolicy();
        
        string camelCaseValue = "helloThere";
        string pascalCaseValue = "HelloThere";
        string newValue = policy.ConvertName(camelCaseValue);

        Assert.Equal(pascalCaseValue, newValue);
    }

    [Fact]
    public void SessionCreationTest()
    {
        SessionTokenManager manager = new SessionTokenManager();

        string username = "User1";
        object token = manager.CreateToken(username);
        
        Assert.IsType<string>(token);
    }
}