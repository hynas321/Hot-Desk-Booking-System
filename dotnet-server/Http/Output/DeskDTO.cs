namespace Dotnet.Server.Http;

public class DeskDTO
{
    public string DeskName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string? Username { get; set; } = string.Empty;
    public string? StartTime { get; set; } = string.Empty;
    public string? EndTime { get; set; } = string.Empty;
}