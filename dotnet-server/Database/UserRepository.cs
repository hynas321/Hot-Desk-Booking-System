using Dotnet.Server.Authentication;
using Dotnet.Server.Http;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Server.Database;

public class UserRepository
{
    private readonly ApplicationDbContext dbContext;

    public UserRepository(ApplicationDbContext dbContext, SessionTokenManager tokenManager)
    {
        this.dbContext = dbContext;
    }

    public void AddUser(UserCredentials userCredentials)
    {
        User newUser = new User()
        {
            Username = userCredentials.Username,
            Password = userCredentials.Password,
            IsAdmin = false
        };

        dbContext.Add(newUser);
        dbContext.SaveChanges();
    }

    public bool DeleteUser(string username)
    {
        User? userToDelete = dbContext.Users.FirstOrDefault(user => user.Username == username);

        if (userToDelete != null)
        {
            dbContext.Users.Remove(userToDelete);
            dbContext.SaveChanges();

            return true;
        }

        return false;
    }

    public bool SetAdminStatus(string username, bool isAdmin)
    {
        User user = dbContext.Users.FirstOrDefault(user => user.Username == username);

        if (user != null)
        {
            user.IsAdmin = isAdmin;

            dbContext.Update(user);
            dbContext.SaveChanges();

            return true;
        }

        return false;
    }

    public bool CheckIfUserExists(string username)
    {
        return dbContext.Users.Any(user => user.Username == username);
    }

    public User GetUser(string username)
    {
        return dbContext.Users.FirstOrDefault(user => user.Username == username);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await dbContext.Users.ToListAsync();
    }
}