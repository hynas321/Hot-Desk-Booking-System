using Microsoft.EntityFrameworkCore;
using WebApi.Models;
using WebApi.Repositories.Abstractions;

namespace WebApi.Repositories;

public class UserRepository : IUserRepository
{
    #nullable disable warnings
    private readonly ApplicationDbContext dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken)
    {
        await dbContext.AddAsync(user, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveUserAsync(User user, CancellationToken cancellationToken)
    {
        dbContext?.Users.Remove(user);
        await dbContext?.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateUserAsync(User user, CancellationToken cancellationToken)
    {
        dbContext.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<User> GetUserAsync(string username, CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .Include(u => u.Bookings)
            .FirstOrDefaultAsync(user => user.Username == username, cancellationToken);
    }

    public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Users
            .Include(u => u.Bookings)
            .ToListAsync(cancellationToken);
    }
}
