using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Desk
{
    [Key]
    public string DeskName { get; set; } = string.Empty;
    [ForeignKey("Location")]
    public string LocationName { get; set; } = string.Empty;
    public string BookerUsername { get; set; } = string.Empty;
    public DateTime BookingStartTime { get; set; }
    public DateTime BookingEndTime { get; set; }
}