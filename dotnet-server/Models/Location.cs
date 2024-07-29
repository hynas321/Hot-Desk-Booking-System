using System.ComponentModel.DataAnnotations;

public class Location
{
    [Key]
    public int Id { get; set; }

    public string LocationName { get; set; } = string.Empty;

    // Navigation property for Desks
    public virtual List<Desk> Desks { get; set; } = new List<Desk>();
}