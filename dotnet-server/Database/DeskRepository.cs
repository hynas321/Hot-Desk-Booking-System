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

            if (desk != null && desk.Username == null)
            {
                dbContext?.Desks.Remove(desk);
                dbContext?.SaveChanges();

                return true;
            }
        }

        return false;
    }

    public bool CheckIfDeskExists(DeskInformation deskInfo)
    {
        if (dbContext.Desks == null)
        {
            throw new NullReferenceException();
        }

        Location? location = dbContext?.Locations?.FirstOrDefault(l => l.LocationName ==deskInfo.LocationName);

        if (location != null)
        {
            return location.Desks.Any(d => d.DeskName == deskInfo.DeskName);
        }

        return false;
    }

    public ClientsideDesk? BookDesk(string username, BookingInformation bookingInformation)
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
                DateTime utcNow = DateTime.UtcNow;
                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, localTimeZone);

                desk.Username = username;
                desk.BookingStartTime = localTime;
                desk.BookingEndTime = localTime.AddDays(bookingInformation.Days);

                dbContext?.Desks.Update(desk);
                dbContext?.SaveChanges();

                string bookingStartTimeString = desk.BookingStartTime.Value.ToString("dd-MM-yyyy HH:mm:ss");
                string bookingEndTimeString = desk.BookingEndTime.Value.ToString("dd-MM-yyyy HH:mm:ss");

                ClientsideDesk clientsideDesk = new ClientsideDesk()
                {
                    DeskName = desk.DeskName,
                    Username = desk.Username,
                    BookingStartTime = bookingStartTimeString,
                    BookingEndTime = bookingEndTimeString
                };

                return clientsideDesk;
            }
        }

        return null;
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