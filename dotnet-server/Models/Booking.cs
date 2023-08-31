using System.ComponentModel.DataAnnotations;

public class Booking
{
    [Key]
    public int Id { get; set; }
    public string? Username { get; set; } = null;
    public DateTime? StartTime { get; set; } = null;
    public DateTime? EndTime { get; set; }
}