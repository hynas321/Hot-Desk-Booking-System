namespace Dotnet.Server.Services;

public interface IUserService
{
    Task<bool> AddUserAsync(User user, CancellationToken cancellationToken);
    Task<bool> RemoveUserAsync(string username, CancellationToken cancellationToken);
    Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken);
    Task<User?> GetUserAsync(string username, CancellationToken cancellationToken);
    Task<List<User>?> GetAllUsersAsync(CancellationToken cancellationToken);
}
