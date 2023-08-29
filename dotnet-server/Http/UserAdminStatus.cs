using System.ComponentModel.DataAnnotations;

namespace Dotnet.Server.Http;

public class UserAdminStatus
{
	[Required]
	public required string Username { get; set; }
	[Required]
	public required bool IsAdmin { get; set; }
}
