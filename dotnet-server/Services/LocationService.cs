using Dotnet.Server.Repositories;

namespace Dotnet.Server.Services;

public class LocationService : ILocationService
{
    private readonly ILocationRepository locationRepository;

    public LocationService(ILocationRepository locationRepository)
    {
        this.locationRepository = locationRepository;
    }

    public async Task<bool> AddLocationAsync(Location location, CancellationToken cancellationToken)
    {
        var existingLocation = await locationRepository.GetLocationAsync(location.LocationName, cancellationToken);

        if (existingLocation != null)
        {
            return false;
        }

        await locationRepository.AddLocationAsync(location, cancellationToken);
        return true;
    }

    public async Task<bool> RemoveLocationAsync(string locationName, CancellationToken cancellationToken)
    {
        var location = await locationRepository.GetLocationAsync(locationName, cancellationToken);

        if (location == null || location.Desks.Count != 0)
        {
            return false;
        }

        await locationRepository.RemoveLocationAsync(location, cancellationToken);
        return true;
    }

    public async Task<Location> GetLocationAsync(string locationName, CancellationToken cancellationToken)
    {
        return await locationRepository.GetLocationAsync(locationName, cancellationToken);
    }

    public async Task<List<Location>> GetAllLocationsAsync(CancellationToken cancellationToken)
    {
        return await locationRepository.GetAllLocationsAsync(cancellationToken);
    }
}
