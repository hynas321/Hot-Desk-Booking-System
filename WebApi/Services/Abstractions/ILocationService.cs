using WebApi.Models;

namespace WebApi.Services.Abstractions;

public interface ILocationService
{
    Task<bool> AddLocationAsync(Location location, CancellationToken cancellationToken);
    Task<bool> RemoveLocationAsync(string locationName, CancellationToken cancellationToken);
    Task<Location> GetLocationAsync(string locationName, CancellationToken cancellationToken);
    Task<List<Location>> GetAllLocationsAsync(CancellationToken cancellationToken);
}
