using Microsoft.EntityFrameworkCore;

namespace Dotnet.Server.Database;

public class LocationRepository
{
    #nullable disable warnings
    private readonly ApplicationDbContext dbContext;

    public LocationRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public void AddLocation(string locationName)
    {
        Location newLocation = new Location()
        {
            LocationName = locationName,
            Desks = new List<Desk>()
        };

        dbContext.Add(newLocation);
        dbContext.SaveChanges();
    }

    public bool RemoveLocation(string locationName)
    {
        if (dbContext.Locations == null)
        {
            throw new NullReferenceException();
        }

        Location? locationToRemove = dbContext?.Locations?.FirstOrDefault(location => location.LocationName == locationName);

        if (locationToRemove != null && locationToRemove.Desks.Count == 0)
        {
            dbContext?.Locations.Remove(locationToRemove);
            dbContext?.SaveChanges();

            return true;
        }

        return false;
    }

    public Location? GetLocation(string locationName)
    {
        if (dbContext.Locations == null)
        {
            throw new NullReferenceException();
        }

        return dbContext.Locations.FirstOrDefault(location => location.LocationName == locationName);
    }

    public async Task<List<Location>> GetAllLocationsAsync()
    {
        if (dbContext.Locations == null)
        {
            throw new NullReferenceException();
        }

        return await dbContext.Locations.ToListAsync();
    }

    public bool CheckIfLocationExists(string locationName)
    {
        if (dbContext.Locations == null)
        {
            throw new NullReferenceException();
        }

        return dbContext.Locations.Any(location => location.LocationName == locationName);
    }
}
