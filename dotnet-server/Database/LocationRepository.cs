using Microsoft.EntityFrameworkCore;

namespace Dotnet.Server.Database;

public class LocationRepository
{
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

    public bool AddDesk(string deskName, string locationName)
    {        
        var location = dbContext?.Locations?.FirstOrDefault(location => location.LocationName == locationName);

        if (location != null)
        {
            Desk desk = new Desk()
            {
                DeskName = deskName,
                Username = null,
                BookingStartTime = null,
                BookingEndTime = null
            };

            location.Desks.Add(desk);
            dbContext?.SaveChanges();

            return true;
        }

        return false;
    }

    public bool RemoveLocation(string locationName)
    {
        if (dbContext.Locations == null)
        {
            throw new NullReferenceException();
        }

        Location? locationToRemove = dbContext?.Locations?.FirstOrDefault(location => location.LocationName == locationName);

        if (locationToRemove != null)
        {
            dbContext?.Locations.Remove(locationToRemove);
            dbContext?.SaveChanges();

            return true;
        }

        return false;
    }

    public bool RemoveDesk(string deskName, string locationName)
    {
        if (dbContext.Desks == null)
        {
            throw new NullReferenceException();
        }

        Location? location = dbContext?.Locations?.FirstOrDefault(location => location.LocationName == locationName);

        if (location != null)
        {
            Desk? desk = location.Desks.FirstOrDefault(desk => desk.DeskName == deskName);

            if (desk != null)
            {
                dbContext?.Desks.Remove(desk);
                dbContext?.SaveChanges();

                return true;
            }
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

    public int GetDeskCountInLocation(string locationName)
    {
        if (dbContext.Locations == null)
        {
            throw new NullReferenceException();
        }

        Location? location = dbContext?.Locations?.FirstOrDefault(location => location.LocationName == locationName);

        if (location != null)
        {
            return location.Desks.Count;
        }

        return -1;
    }
}
