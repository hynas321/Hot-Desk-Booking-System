using Dotnet.Server.Http;

namespace Dotnet.Server.Repositories;

public interface IDeskRepository
{
    Task<bool> AddDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken);
    Task<bool> RemoveDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken);
    Task<ClientsideDesk?> SetDeskAvailabilityAsync(DeskInformation deskInformation, bool isEnabled, CancellationToken cancellationToken);
    Task<Desk> GetDeskAsync(DeskInformation deskInfo, CancellationToken cancellationToken);
    Task<List<Desk>> GetAllDesksAsync(CancellationToken cancellationToken);
    List<Desk> GetAllDesks();
}
