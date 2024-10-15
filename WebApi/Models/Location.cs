using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;

public class Location
{
    [Key]
    public int Id { get; set; }

    public string LocationName { get; set; } = string.Empty;

    public List<Desk> Desks { get; set; } = new List<Desk>();
}