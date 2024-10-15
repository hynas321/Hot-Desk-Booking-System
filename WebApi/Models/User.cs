using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WebApi.Models;
public class User
{
    [Key]
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;

    public string Role { get; set; } = "User";

    [JsonIgnore]
    public virtual List<Booking> Bookings { get; set; } = new List<Booking>();
}