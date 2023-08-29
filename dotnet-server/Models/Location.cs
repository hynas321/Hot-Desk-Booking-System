using System.ComponentModel.DataAnnotations;

public class Location
{
    [Key]
    public string LocationName { get; set; } = string.Empty;
    public List<Desk> Desks { get; set; } = new List<Desk>();
}