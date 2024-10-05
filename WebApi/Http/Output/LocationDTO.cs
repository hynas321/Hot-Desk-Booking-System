namespace WebApi.Http;

public class LocationDTO
{
	public string LocationName { get; set; } = string.Empty;
    public int TotalDeskCount { get; set; }
    public int AvailableDeskCount { get; set; }
}