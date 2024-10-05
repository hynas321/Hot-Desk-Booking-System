using WebApi.Http;
using WebApi.Models;

namespace WebApi.Repositories.Abstractions;

public interface IDeskRepository
{
    Task AddDeskAsync(Desk desk, CancellationToken cancellationToken);
    Task RemoveDeskAsync(Desk desk, CancellationToken cancellationToken);
    Task UpdateDeskAsync(Desk desk, CancellationToken cancellationToken);
    Task<Desk> GetDeskAsync(DeskInformation deskInfo, CancellationToken cancellationToken);
    Task<List<Desk>> GetAllDesksAsync(CancellationToken cancellationToken);
    List<Desk> GetAllDesks();
}
