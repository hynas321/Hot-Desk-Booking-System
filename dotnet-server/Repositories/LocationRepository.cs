using Microsoft.EntityFrameworkCore;

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

    public async Task RemoveLocationAsync(Location location, CancellationToken cancellationToken)
    {
        dbContext?.Locations.Remove(location);
        await dbContext?.SaveChangesAsync(cancellationToken);
    }

    public async Task<Location> GetLocationAsync(string locationName, CancellationToken cancellationToken)
    {
        return await dbContext.Locations.FirstOrDefaultAsync(location => location.LocationName == locationName, cancellationToken);
    }

    public async Task<List<Location>> GetAllLocationsAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Locations.ToListAsync(cancellationToken);
    }
}
