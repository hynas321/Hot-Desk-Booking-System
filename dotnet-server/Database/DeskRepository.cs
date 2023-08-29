using Dotnet.Server.Http;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Server.Database;

public class DeskRepository
{
    private readonly ApplicationDbContext dbContext;

    public DeskRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public bool AddDesk(DeskInformation deskInformation)
    {        
        var location = dbContext?.Locations?.FirstOrDefault(l => l.LocationName == deskInformation.LocationName);

        if (location != null)
        {
            Desk desk = new Desk()
            {
                DeskName = deskInformation.DeskName,
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

    public bool RemoveDesk(DeskInformation deskInformation)
    {
        if (dbContext.Desks == null)
        {
            throw new NullReferenceException();
        }

        Location? location = dbContext?.Locations?.FirstOrDefault(l => l.LocationName == deskInformation.LocationName);

        if (location != null)
        {
            Desk? desk = location.Desks.FirstOrDefault(d => d.DeskName == deskInformation.DeskName);

            if (desk != null)
            {
                dbContext?.Desks.Remove(desk);
                dbContext?.SaveChanges();

                return true;
            }
        }

        return false;
    }

    public bool BookDesk(string username, BookingInformation bookingInformation)
    {
        if (dbContext.Desks == null)
        {
            throw new NullReferenceException();
        }

        Location? location = dbContext?.Locations?.FirstOrDefault(l => l.LocationName == bookingInformation.LocationName);

        if (location != null)
        {
            Desk? desk = location.Desks.FirstOrDefault(d => d.DeskName == bookingInformation.DeskName);

            if (desk != null && desk.Username == null)
            {
                DateTime now = DateTime.UtcNow;

                desk.Username = username;
                desk.BookingStartTime = now;
                desk.BookingEndTime = now.AddDays(bookingInformation.Days);

                dbContext?.Desks.Update(desk);
                dbContext?.SaveChanges();

                return true;
            }
        }

        return false;
    }

    public bool UnbookDesk(DeskInformation deskInformation)
    {
        if (dbContext.Desks == null)
        {
            throw new NullReferenceException();
        }

        Location? location = dbContext?.Locations?.FirstOrDefault(l => l.LocationName == deskInformation.LocationName);

        if (location != null)
        {
            Desk? desk = location.Desks.FirstOrDefault(d => d.DeskName == deskInformation.DeskName);

            if (desk != null && desk.Username != null)
            {
                desk.Username = null;
                desk.BookingStartTime = null;
                desk.BookingEndTime = null;

                dbContext?.Desks.Update(desk);
                dbContext?.SaveChanges();

                return true;
            }
        }

        return false;
    }

    public async Task<List<Desk>> GetAllDesksAsync()
    {
        if (dbContext.Desks == null)
        {
            throw new NullReferenceException();
        }

        return await dbContext.Desks.ToListAsync();
    }
}