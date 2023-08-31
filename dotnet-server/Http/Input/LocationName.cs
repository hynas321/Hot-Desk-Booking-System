using System.ComponentModel.DataAnnotations;

namespace Dotnet.Server.Http;

public class LocationName
{
	[Required]
	public string Name { get; set; } = string.Empty;
}
