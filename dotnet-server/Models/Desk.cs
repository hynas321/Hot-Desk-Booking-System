using System.ComponentModel.DataAnnotations;

public class Desk
{
    [Key]
    public string DeskName { get; set; } = string.Empty;
    public string? Username { get; set; } = null;
    public DateTime? BookingStartTime { get; set; } = null;
    public DateTime? BookingEndTime { get; set; }
}