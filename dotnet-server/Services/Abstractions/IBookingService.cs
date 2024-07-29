using Dotnet.Server.Http;

namespace Dotnet.Server.Services;

public interface IBookingService
{
    Task<DeskDTO?> AddBookingAsync(User user, BookingInformation bookingInformation, CancellationToken cancellationToken);
    Task<DeskDTO?> RemoveBookingAsync(DeskInformation deskInformation, CancellationToken cancellationToken);
}
