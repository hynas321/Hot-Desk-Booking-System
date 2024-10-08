using System.ComponentModel.DataAnnotations;

namespace WebApi.Http;

public class UserCredentials
{
	[Required]
	[MinLength(5)]
	public string Username { get; set; } = string.Empty;
	[Required]
	[MinLength(5)]
	public string Password { get; set; } = string.Empty;
}
