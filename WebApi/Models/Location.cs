using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.Models;

public class Location
{
    [Key]
    public int Id { get; set; }

    public string LocationName { get; set; } = string.Empty;

    [JsonIgnore]
    public virtual List<Desk> Desks { get; set; } = new List<Desk>();
}