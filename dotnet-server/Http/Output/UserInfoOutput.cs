namespace Dotnet.Server.Http;

public class UserInfoOutput {
    public string? Username { get; set; }
    public bool IsAdmin { get; set; }
    public ClientsideDesk? BookedDesk { get; set; }
    public string? BookedDeskLocation { get; set; }
}