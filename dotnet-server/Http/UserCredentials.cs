using System.ComponentModel.DataAnnotations;

namespace Dotnet.Server.Http;

public class UserCredentials
{
	[Required]
	[MinLength(5)]
	[MaxLength(20)]
	public required string Username { get; set; }
	[Required]
	[MinLength(5)]
	public required string Password { get; set; }
}
