using WebApi.Http;
using WebApi.Models;

namespace WebApi.Repositories.Abstractions;

public interface IBookingRepository
{
    Task AddBookingAsync(Booking booking, CancellationToken cancellationToken);
    Task RemoveBookingAsync(Booking booking, CancellationToken cancellationToken);
    Task<Booking?> GetBookingAsync(DeskInformation deskInformation, CancellationToken cancellationToken);
}

