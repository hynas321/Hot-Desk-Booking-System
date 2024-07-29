namespace Dotnet.Server.Repositories;

public interface ILocationRepository
{
    Task AddLocationAsync(Location location, CancellationToken cancellationToken);
    Task RemoveLocationAsync(Location location, CancellationToken cancellationToken);
    Task<Location> GetLocationAsync(string locationName, CancellationToken cancellationToken);
    Task<List<Location>> GetAllLocationsAsync(CancellationToken cancellationToken);
}
