using System.ComponentModel.DataAnnotations;

namespace WebApi.Http;

public class DeskAvailabilityInformation
{
    [Required]
    public string DeskName { get; set; } = string.Empty;
    [Required]
    public string LocationName { get; set; } = string.Empty;
    [Required]
    public bool IsEnabled { get; set; }
}
