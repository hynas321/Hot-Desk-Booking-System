using System.Net;
using System.Net.Http.Headers;
using System.Text;
using DatabaseProject.Test.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Helpers;
using WebApi.Http;
using WebApi.Managers.Abstractions;
using WebApi.Models;
using WebApi.Models.Constants;
using WebApi.Repositories;
using Xunit;

namespace WebApi.Tests.Integration
{
    public class UserControllerTests : IClassFixture<DockerWebApplicationFactoryFixture>
    {
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly ISessionTokenManager _sessionTokenManager;
        private readonly ApplicationDbContext _dbContext;

        public UserControllerTests(DockerWebApplicationFactoryFixture factory)
        {
            _client = factory.CreateClient();
            _scope = factory.Services.CreateScope();

            _sessionTokenManager = _scope.ServiceProvider.GetRequiredService<ISessionTokenManager>();
            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>()
                ?? throw new Exception("DbContext does not exist");
        }

        [Fact]
        public async Task AddUser_ShouldReturnCreated_WhenUserIsAddedSuccessfully()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var userCredentials = new UserCredentials { Username = $"testuser_{uniqueSuffix}", Password = "password123" };
            var serializedUserCredentials = JsonHelper.Serialize(userCredentials);
            var requestContent = new StringContent(serializedUserCredentials, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync("/api/user/add", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task AddUser_ShouldReturnConflict_WhenUserAlreadyExists()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var user = new User { Username = $"testuser_{uniqueSuffix}", Password = "password123" };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var userCredentials = new UserCredentials { Username = user.Username, Password = "password123" };
            var serializedUserCredentials = JsonHelper.Serialize(userCredentials);
            var requestContent = new StringContent(serializedUserCredentials, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync("/api/user/add", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task RemoveUser_ShouldReturnOk_WhenUserIsRemovedSuccessfully()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var user = new User { Username = $"testuser_{uniqueSuffix}", Password = "password123" };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var serializedUsername = JsonHelper.Serialize(user.Username);
            var requestContent = new StringContent(serializedUsername, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/user/remove")
            {
                Content = requestContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LogIn_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var hashManager = _scope.ServiceProvider.GetRequiredService<IHashManager>();

            var user = new User
            {
                Username = $"testuser_{uniqueSuffix}",
                Password = hashManager.HashPassword("password123")
            };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var userCredentials = new UserCredentials { Username = user.Username, Password = "password123" };
            var serializedUserCredentials = JsonHelper.Serialize(userCredentials);
            var requestContent = new StringContent(serializedUserCredentials, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/user/login", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task LogIn_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var userCredentials = new UserCredentials { Username = $"nonexistentuser_{uniqueSuffix}", Password = "password123" };
            var serializedUserCredentials = JsonHelper.Serialize(userCredentials);
            var requestContent = new StringContent(serializedUserCredentials, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/user/login", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task SetAdmin_ShouldReturnOk_WhenAdminStatusIsUpdated()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var user = new User { Username = $"testuser_{uniqueSuffix}", Password = "password123", Role = UserRole.User };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var userAdminStatus = new UserAdminStatus { Username = user.Username, IsAdmin = true };
            var serializedUserAdminStatus = JsonHelper.Serialize(userAdminStatus);
            var requestContent = new StringContent(serializedUserAdminStatus, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync("/api/user/setadmin", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task IsAdmin_ShouldReturnOk_WhenUserIsAdmin()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var user = new User { Username = $"adminuser_{uniqueSuffix}", Password = "password123", Role = UserRole.Admin };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"/api/user/isadmin/{user.Username}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetUserInfo_ShouldReturnOk_WhenUserIsAuthenticated()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var user = new User { Username = $"testuser_{uniqueSuffix}", Password = "password123" };
            await _dbContext.Users.AddAsync(user);
            await _dbContext.SaveChangesAsync();

            var token = _sessionTokenManager.CreateToken(user.Username, "User");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync("/api/user/getuserinfo");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
