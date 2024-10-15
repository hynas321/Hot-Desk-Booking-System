using System.ComponentModel.DataAnnotations;

namespace WebApi.Models;
public class User
{
    [Key]
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;

    public string Role { get; set; } = "User";

    public List<Booking> Bookings { get; set; } = new List<Booking>();
}