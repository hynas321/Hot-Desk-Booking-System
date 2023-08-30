using System.ComponentModel.DataAnnotations;

namespace Dotnet.Server.Http;

public class DeskInformation
{
	[Required]
	public string DeskName { get; set; } = string.Empty;
    [Required]
    public string LocationName { get; set; } = string.Empty;
}
