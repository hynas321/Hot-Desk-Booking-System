using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Desk
{
    [Key]
    public int Id { get; set; }

    public string DeskName { get; set; } = string.Empty;

    public bool IsEnabled { get; set; }

    public int LocationId { get; set; }

    [ForeignKey("LocationId")]
    public Location Location { get; set; } = new Location();

    public List<Booking> Bookings { get; set; } = new List<Booking>();
}