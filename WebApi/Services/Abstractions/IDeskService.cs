using WebApi.Http;
using WebApi.Models;

namespace WebApi.Services.Abstractions;

public interface IDeskService
{
    Task<bool> AddDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken);
    Task<bool> RemoveDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken);
    Task<DeskDTO?> SetDeskAvailabilityAsync(DeskInformation deskInformation, bool isEnabled, CancellationToken cancellationToken);
    Task<Desk?> GetDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken);
    Task<List<Desk>> GetAllDesksAsync(CancellationToken cancellationToken);
    List<Desk> GetAllDesks();
}
