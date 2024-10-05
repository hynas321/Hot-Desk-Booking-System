using WebApi.Http;
using WebApi.Models;

namespace WebApi.Services.Abstractions;

public interface IBookingService
{
    Task<DeskDTO?> AddBookingAsync(User user, BookingInformation bookingInformation, CancellationToken cancellationToken);
    Task<DeskDTO?> RemoveBookingAsync(DeskInformation deskInformation, CancellationToken cancellationToken);
}
