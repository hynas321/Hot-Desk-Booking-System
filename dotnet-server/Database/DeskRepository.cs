using Dotnet.Server.Http;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Server.Database;

public class DeskRepository
{
    #nullable disable warnings
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
                IsEnabled = true,
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

    public ClientsideDesk? SetDeskAvailability(DeskInformation deskInformation, bool isEnabled)
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
                desk.IsEnabled = isEnabled;
                dbContext?.Desks.Update(desk);
                dbContext?.SaveChanges();

                ClientsideDesk clientsideDesk = new ClientsideDesk()
                {
                    DeskName = desk.DeskName,
                    IsEnabled = desk.IsEnabled,
                    Username = null,
                    StartTime = null,
                    EndTime = null
                };

                return clientsideDesk;
            }
        }

        return null;
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

    public List<Desk> GetAllDesks()
    {
        if (dbContext.Desks == null)
        {
            throw new NullReferenceException();
        }

        return dbContext.Desks.ToList();
    }
}