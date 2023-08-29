using Dotnet.Server.Authentication;
using Dotnet.Server.Http;
using Microsoft.EntityFrameworkCore;

namespace Dotnet.Server.Database;

public class UserRepository
{
    private readonly ApplicationDbContext dbContext;

    public UserRepository(ApplicationDbContext dbContext)
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

    public bool RemoveUser(string username)
    {
        if (dbContext.Users == null)
        {
            throw new NullReferenceException();
        }

        User? userToRemove = dbContext?.Users?.FirstOrDefault(user => user.Username == username);

        if (userToRemove != null)
        {
            dbContext?.Users.Remove(userToRemove);
            dbContext?.SaveChanges();

            return true;
        }

        return false;
    }

    public User? GetUser(string username)
    {
        if (dbContext.Users == null)
        {
            throw new NullReferenceException();
        }

        return dbContext.Users.FirstOrDefault(user => user.Username == username);
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        if (dbContext.Users == null)
        {
            throw new NullReferenceException();
        }

        return await dbContext.Users.ToListAsync();
    }

    public bool CheckIfUserExists(string username)
    {
        if (dbContext.Users == null)
        {
            throw new NullReferenceException();
        }

        return dbContext.Users.Any(user => user.Username == username);
    }

    public bool SetAdminStatus(string username, bool isAdmin)
    {
        if (dbContext.Users == null)
        {
            throw new NullReferenceException();
        }

        User? user = dbContext.Users.FirstOrDefault(user => user.Username == username);

        if (user != null)
        {
            user.IsAdmin = isAdmin;

            dbContext.Update(user);
            dbContext.SaveChanges();

            return true;
        }

        return false;
    }
}