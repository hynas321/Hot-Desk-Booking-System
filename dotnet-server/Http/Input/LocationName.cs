using System.ComponentModel.DataAnnotations;

namespace Dotnet.Server.Http;

public class LocationName
{
	[Required]
	[MinLength(1)]
	public string Name { get; set; } = string.Empty;
}
