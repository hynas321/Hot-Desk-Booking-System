using Dotnet.Server.Http;

namespace Dotnet.Server.Repositories;

public interface IBookingRepository
{
    Task AddBookingAsync(Booking booking, CancellationToken cancellationToken);
    Task RemoveBookingAsync(Booking booking, CancellationToken cancellationToken);
    Task<Booking?> GetBookingAsync(DeskInformation deskInformation, CancellationToken cancellationToken);
}

