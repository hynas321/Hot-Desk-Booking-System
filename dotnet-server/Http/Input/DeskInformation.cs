using System.ComponentModel.DataAnnotations;

namespace Dotnet.Server.Http;

public class DeskInformation
{
	[Required]
	[MinLength(1)]
	public string DeskName { get; set; } = string.Empty;
    [Required]
    [MinLength(1)]
    public string LocationName { get; set; } = string.Empty;
}
