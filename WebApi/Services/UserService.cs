using WebApi.Models;
using WebApi.Repositories.Abstractions;
using WebApi.Services.Abstractions;

namespace WebApi.Services;

public class UserService : IUserService
{
    private readonly IUserRepository userRepository;

    public UserService(IUserRepository userRepository)
    {
        this.userRepository = userRepository;
    }

    public async Task<bool> AddUserAsync(User user, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetUserAsync(user.Username, cancellationToken);

        if (existingUser != null)
        {
            return false;
        }

        await userRepository.AddUserAsync(user, cancellationToken);
        return true;
    }

    public async Task<bool> RemoveUserAsync(string username, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetUserAsync(username, cancellationToken);

        if (user == null)
        {
            return false;
        }

        await userRepository.RemoveUserAsync(user, cancellationToken);
        return true;
    }

    public async Task<bool> UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        var existingUser = await userRepository.GetUserAsync(user.Username, cancellationToken);

        if (existingUser == null)
        {
            return false;
        } 

        await userRepository.UpdateUserAsync(user, cancellationToken);
        return true;
    }

    public async Task<User?> GetUserAsync(string username, CancellationToken cancellationToken)
    {
        return await userRepository.GetUserAsync(username, cancellationToken);
    }

    public async Task<List<User>?> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        return await userRepository.GetAllUsersAsync(cancellationToken);
    }
}