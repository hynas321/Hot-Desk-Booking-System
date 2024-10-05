using System.ComponentModel.DataAnnotations;

namespace WebApi.Http;

public class LocationName
{
	[Required]
	public string Name { get; set; } = string.Empty;
}
