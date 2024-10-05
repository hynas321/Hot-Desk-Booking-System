using System.ComponentModel.DataAnnotations;

namespace WebApi.Http;

public class BookingInformation
{
	[Required]
	public string DeskName { get; set; } = string.Empty;
    [Required]
    public string LocationName { get; set; } = string.Empty;
    [Required]
    public int Days { get; set; }
}
