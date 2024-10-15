using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApi.Models;

public class Desk
{
    [Key]
    public int Id { get; set; }

    public string DeskName { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }

    public int LocationId { get; set; }

    [ForeignKey("LocationId")]
    [JsonIgnore]
    public virtual Location Location { get; set; }

    [JsonIgnore]
    public virtual List<Booking> Bookings { get; set; } = new List<Booking>();
}