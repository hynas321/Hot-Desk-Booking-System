using Dotnet.Server.Http;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace Dotnet.Server.Repositories;

public class DeskRepository : IDeskRepository
{
    #nullable disable warnings
    private readonly ApplicationDbContext dbContext;

    public DeskRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<bool> AddDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken)
    {        
        Location location = await dbContext?.Locations?.FirstOrDefaultAsync(l => l.LocationName == deskInformation.LocationName, cancellationToken);

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
            await dbContext?.SaveChangesAsync(cancellationToken);

            return true;
        }

        return false;
    }

    public async Task<bool> RemoveDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken)
    {
        Location? location = await dbContext?.Locations?.FirstOrDefaultAsync(l => l.LocationName == deskInformation.LocationName, cancellationToken);

        if (location != null)
        {
            Desk? desk = location.Desks.FirstOrDefault(d => d.DeskName == deskInformation.DeskName);

            if (desk != null && desk?.Booking?.Username == null)
            {
                dbContext?.Desks.Remove(desk);
                await dbContext?.SaveChangesAsync(cancellationToken);

                return true;
            }
        }

        return false;
    }

    public async Task<ClientsideDesk?> SetDeskAvailabilityAsync(DeskInformation deskInformation, bool isEnabled, CancellationToken cancellationToken)
    {
        Location? location = await dbContext?.Locations?.FirstOrDefaultAsync(l => l.LocationName == deskInformation.LocationName, cancellationToken);

        if (location != null)
        {
            Desk? desk = location.Desks.FirstOrDefault(d => d.DeskName == deskInformation.DeskName);

            if (desk != null && desk?.Booking?.Username == null)
            {   
                desk.IsEnabled = isEnabled;
                dbContext?.Desks.Update(desk);
                await dbContext?.SaveChangesAsync(cancellationToken);

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

    public async Task<Desk> GetDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken)
    {
        Location? location = await dbContext?.Locations?.FirstOrDefaultAsync(l => l.LocationName == deskInformation.LocationName, cancellationToken);

        if (location == null)
        {
            return null;
        }

        return location.Desks.FirstOrDefault(d => d.DeskName == deskInformation.DeskName);
    }

    public async Task<List<Desk>> GetAllDesksAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Desks.ToListAsync();
    }

    public List<Desk> GetAllDesks()
    {
        return dbContext.Desks.ToList();
    }
}