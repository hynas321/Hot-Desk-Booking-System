using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Booking
{
    [Key]
    public int Id { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    // Foreign key for User
    public string? UserId { get; set; }

    // Foreign key for Desk
    public int DeskId { get; set; }

    // Navigation property for Desk
    [ForeignKey("DeskId")]
    public virtual Desk? Desk { get; set; }

    // Navigation property for User
    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}