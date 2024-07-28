using Dotnet.Server.Http;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Server.Repositories;

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

    public async Task<bool> RemoveUserAsync(string username, CancellationToken cancellationToken)
    {
        User? userToRemove = dbContext?.Users?.FirstOrDefault(user => user.Username == username);

        if (userToRemove == null)
        {
            return false;
        }

        dbContext?.Users.Remove(userToRemove);
        await dbContext?.SaveChangesAsync(cancellationToken);

        return true;
    }

    public async Task<User?> GetUserAsync(string username, CancellationToken cancellationToken)
    {
        return await dbContext.Users.FirstOrDefaultAsync(user => user.Username == username, cancellationToken);
    }

    public async Task<List<User>> GetAllUsersAsync(CancellationToken cancellationToken)
    {
        return await dbContext.Users.ToListAsync(cancellationToken);
    }

    public async Task<bool> CheckIfUserExistsAsync(string username, CancellationToken cancellationToken)
    {
        return await dbContext.Users.AnyAsync(user => user.Username == username, cancellationToken);
    }

    public async Task<DeskInformation?> GetBookedDeskInformationAsync(string username, CancellationToken cancellationToken)
    {
        Desk? desk = await dbContext?.Desks?.FirstOrDefaultAsync(d => d.Booking.Username == username, cancellationToken);

        if (desk == null)
        {
            return null;
        }

        DeskInformation booking = new DeskInformation()
        {
            DeskName = desk.DeskName,
            LocationName = desk.Location.LocationName
        };

        return booking;
    }

    public async Task<bool> SetAdminStatus(string username, bool isAdmin, CancellationToken cancellationToken)
    {
        User? user = dbContext.Users.FirstOrDefault(user => user.Username == username);

        if (user == null)
        {
            return false;
        }

        user.IsAdmin = isAdmin;

        dbContext.Update(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}