using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Dotnet.Server.Repositories;

public class LocationRepository : ILocationRepository
{
    #nullable disable warnings
    private readonly ApplicationDbContext dbContext;

    public LocationRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddLocationAsync(Location location, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(location, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> RemoveLocationAsync(string locationName, CancellationToken cancellationToken)
    {
        Location? locationToRemove = await dbContext?.Locations?.FirstOrDefaultAsync(location => location.LocationName == locationName, cancellationToken);

        if (locationToRemove == null || locationToRemove.Desks.Count != 0)
        {
            return false;
        }

        dbContext?.Locations.Remove(locationToRemove);
        await dbContext?.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<Location?> GetLocationAsync(string locationName, CancellationToken cancellationToken)
    {
        return await dbContext.Locations.FirstOrDefaultAsync(location => location.LocationName == locationName, cancellationToken);
    }

    public async Task<List<Location>> GetAllLocationsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Locations.ToListAsync(cancellationToken);
    }

    public async Task<bool> CheckIfLocationExists(string locationName, CancellationToken cancellationToken)
    {
        return await dbContext.Locations.AnyAsync(location => location.LocationName == locationName, cancellationToken);
    }
}
