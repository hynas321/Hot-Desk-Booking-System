using WebApi.Http;
using Microsoft.EntityFrameworkCore;
using WebApi.Repositories.Abstractions;
using WebApi.Models;

namespace WebApi.Repositories;

public class DeskRepository : IDeskRepository
{
    #nullable disable warnings
    private readonly ApplicationDbContext dbContext;

    public DeskRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddDeskAsync(Desk desk, CancellationToken cancellationToken)
    {        
        await dbContext.Desks.AddAsync(desk, cancellationToken);
        await dbContext?.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveDeskAsync(Desk desk, CancellationToken cancellationToken)
    {
        dbContext?.Desks.Remove(desk);
        await dbContext?.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateDeskAsync(Desk desk, CancellationToken cancellationToken)
    {
        dbContext?.Desks.Update(desk);
        await dbContext?.SaveChangesAsync(cancellationToken);
    }

    public async Task<Desk> GetDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken)
    {
        return await dbContext.Desks.FirstOrDefaultAsync(
            d => d.DeskName == deskInformation.DeskName &&
            d.Location.LocationName == deskInformation.LocationName);
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