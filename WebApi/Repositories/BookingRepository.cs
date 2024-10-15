using WebApi.Http;
using Microsoft.EntityFrameworkCore;
using WebApi.Repositories.Abstractions;
using WebApi.Models;

namespace WebApi.Repositories;

public class BookingRepository : IBookingRepository
{
    #nullable disable warnings
    private readonly ApplicationDbContext dbContext;

    public BookingRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddBookingAsync(Booking booking, CancellationToken cancellationToken)
    {
        await dbContext.Bookings.AddAsync(booking, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveBookingAsync(Booking booking, CancellationToken cancellationToken)
    {
        dbContext.Bookings.Remove(booking);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<Booking?> GetBookingAsync(DeskInformation deskInformation, CancellationToken cancellationToken)
    {
        return await dbContext.Bookings
            .Include(b => b.Desk)
                .ThenInclude(d => d.Location)
            .Include(b => b.User)
            .FirstOrDefaultAsync(
                b => b.Desk.DeskName == deskInformation.DeskName &&
                     b.Desk.Location.LocationName == deskInformation.LocationName, cancellationToken);
    }
}
