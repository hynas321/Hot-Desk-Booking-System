using System.ComponentModel.DataAnnotations;

namespace WebApi.Http;

public class UserAdminStatus
{
	[Required]
	[MinLength(5)]
	public string Username { get; set; } = string.Empty;
	[Required]
	public bool IsAdmin { get; set; }
}
