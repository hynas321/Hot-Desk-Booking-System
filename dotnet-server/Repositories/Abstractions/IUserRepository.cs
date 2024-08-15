namespace Dotnet.Server.Repositories;

public interface IUserRepository
{
    Task AddUserAsync(User user, CancellationToken cancellationToken);
    Task RemoveUserAsync(User user, CancellationToken cancellationToken);
    Task UpdateUserAsync(User user, CancellationToken cancellationToken);
    Task<User> GetUserAsync(string username, CancellationToken cancellationToken);
    Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken);
}
