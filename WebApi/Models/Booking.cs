using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
    [JsonIgnore]
    public virtual Desk? Desk { get; set; }

    [ForeignKey("UserId")]
    [JsonIgnore]
    public virtual User? User { get; set; }
}