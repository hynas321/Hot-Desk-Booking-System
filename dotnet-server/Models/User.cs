using Microsoft.AspNetCore.Identity;

public class User : IdentityUser
{
    public string Password { get; set; }
    // Additional properties for your application user
    public bool IsAdmin { get; set; } = false;

    // Navigation property for Bookings
    public virtual List<Booking> Bookings { get; set; } = new List<Booking>();
}