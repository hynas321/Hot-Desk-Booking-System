using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

public class Desk
{
    [Key]
    public int Id { get; set; }

    public string DeskName { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }

    // Foreign key for Location
    public int LocationId { get; set; }

    // Navigation property for Location
    [ForeignKey("LocationId")]
    [JsonIgnore]
    public virtual Location Location { get; set; }

    // Navigation property for Bookings
    [JsonIgnore]
    public virtual List<Booking> Bookings { get; set; } = new List<Booking>();
}