using Dotnet.Server.Http;

namespace Dotnet.Server.Repositories;

public interface IBookingRepository
{
    Task<ClientsideDesk?> BookDeskAsync(string username, BookingInformation bookingInformation, CancellationToken cancellationToken);
    Task<ClientsideDesk?> UnBookDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken);
}

