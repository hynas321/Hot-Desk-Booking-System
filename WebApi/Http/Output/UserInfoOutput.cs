namespace WebApi.Http;

public class UserInfoOutput {
    public string? Username { get; set; }
    public bool IsAdmin { get; set; }
    public DeskDTO? BookedDesk { get; set; }
    public string? BookedDeskLocation { get; set; }
}