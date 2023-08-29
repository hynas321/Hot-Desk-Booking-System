using System.ComponentModel.DataAnnotations;

namespace Dotnet.Server.Http;

public class UserUsername
{
	[Required]
	[MinLength(5)]
	public string Username { get; set; } = string.Empty;
}
