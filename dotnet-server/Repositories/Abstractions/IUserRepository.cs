using Dotnet.Server.Http;

namespace Dotnet.Server.Repositories;

public interface IUserRepository
{
    Task AddUserAsync(User user, CancellationToken cancellationToken);
    Task<bool> RemoveUserAsync(string username, CancellationToken cancellationToken);
    Task<User?> GetUserAsync(string username, CancellationToken cancellationToken);
    Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken);
    Task<bool> SetAdminStatus(string username, bool isAdmin, CancellationToken cancellationToken);
    Task<DeskInformation?> GetBookedDeskInformationAsync(string username, CancellationToken cancellationToken);
}
