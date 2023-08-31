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
                Booking = new Booking()
                {
                    Username = null,
                    StartTime = null,
                    EndTime = null
                }
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

            if (desk != null && desk?.Booking?.Username == null)
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

            List<Desk> desks = GetAllDesks();
            bool existingAnyUserBookings = desks.Any(d => d.Booking?.Username == username);

            if (desk != null && desk?.Booking?.Username == null && !existingAnyUserBookings)
            {
                DateTime utcNow = DateTime.UtcNow;
                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, localTimeZone);

                desk.Booking.Username = username;
                desk.Booking.StartTime = localTime;
                desk.Booking.EndTime = localTime.AddDays(bookingInformation.Days - 1);

                dbContext?.Desks.Update(desk);
                dbContext?.SaveChanges();

                string bookingStartTimeString = desk.Booking.StartTime.Value.ToString("dd-MM-yyyy");
                string bookingEndTimeString = desk.Booking.EndTime.Value.ToString("dd-MM-yyyy");

                ClientsideDesk clientsideDesk = new ClientsideDesk()
                {
                    DeskName = desk.DeskName,
                    Username = desk.Booking.Username,
                    StartTime = bookingStartTimeString,
                    EndTime = bookingEndTimeString
                };

                return clientsideDesk;
            }
        }

        return null;
    }

    public ClientsideDesk? UnbookDesk(DeskInformation deskInformation)
    {
        if (dbContext.Desks == null)
        {
            throw new NullReferenceException();
        }

        Location? location = dbContext?.Locations?.FirstOrDefault(l => l.LocationName == deskInformation.LocationName);

        if (location != null)
        {
            Desk? desk = location.Desks.FirstOrDefault(d => d.DeskName == deskInformation.DeskName);

            if (desk != null && desk?.Booking?.Username != null)
            {
                desk.Booking.Username = null;
                desk.Booking.StartTime = null;
                desk.Booking.EndTime = null;

                dbContext?.Desks.Update(desk);
                dbContext?.SaveChanges();

                ClientsideDesk clientsideDesk = new ClientsideDesk()
                {
                    DeskName = desk.DeskName,
                    Username = null,
                    StartTime = null,
                    EndTime = null
                };

                return clientsideDesk;
            }
        }

        return null;
    }

    public async Task<List<Desk>> GetAllDesksAsync()
    {
        if (dbContext.Desks == null)
        {
            throw new NullReferenceException();
        }

        return await dbContext.Desks.ToListAsync();
    }

    public List<Desk> GetAllDesks()
    {
        if (dbContext.Desks == null)
        {
            throw new NullReferenceException();
        }

        return dbContext.Desks.ToList();
    }
}