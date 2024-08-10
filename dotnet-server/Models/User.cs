using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

public class User : IdentityUser
{
    public string Password { get; set; }
    // Additional properties for your application user
    public bool IsAdmin { get; set; } = false;

    // Navigation property for Bookings
    [JsonIgnore]
    public virtual List<Booking> Bookings { get; set; } = new List<Booking>();
}