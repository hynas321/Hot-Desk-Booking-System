using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

public class User
{
    [Key]
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
}