using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApi.Models;

public class Booking
{
    [Key]
    public int Id { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public string? UserId { get; set; }

    public int DeskId { get; set; }

    [ForeignKey("DeskId")]
    public Desk? Desk { get; set; }

    [ForeignKey("UserId")]
    public User? User { get; set; }
}