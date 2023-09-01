using System.ComponentModel.DataAnnotations;

public class Desk
{
    [Key]
    public int Id { get; set; }
    public string DeskName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public virtual Booking? Booking { get; set; }
    public virtual Location? Location { get; set; }
}