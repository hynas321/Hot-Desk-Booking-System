using System.Net;
using System.Net.Http.Headers;
using System.Text;
using DatabaseProject.Test.Fixtures;
using Microsoft.Extensions.DependencyInjection;
using WebApi.Helpers;
using WebApi.Http;
using WebApi.Managers.Abstractions;
using WebApi.Models;
using WebApi.Repositories;
using Xunit;

namespace WebApi.Tests.Integration
{
    public class LocationControllerTests : IClassFixture<DockerWebApplicationFactoryFixture>
    {
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly ISessionTokenManager _sessionTokenManager;
        private readonly ApplicationDbContext _dbContext;

        public LocationControllerTests(DockerWebApplicationFactoryFixture factory)
        {
            _client = factory.CreateClient();
            _scope = factory.Services.CreateScope();

            _sessionTokenManager = _scope.ServiceProvider.GetRequiredService<ISessionTokenManager>();
            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>()
                ?? throw new Exception("DbContext does not exist");
        }

        [Fact]
        public async Task AddLocation_ShouldReturnCreated_WhenLocationIsAddedSuccessfully()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var locationName = new LocationName { Name = $"Location_{uniqueSuffix}" };
            var serializedLocationName = JsonHelper.Serialize(locationName);
            var requestContent = new StringContent(serializedLocationName, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync("/api/location/add", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task AddLocation_ShouldReturnConflict_WhenLocationAlreadyExists()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var location = new Location { LocationName = $"Location_{uniqueSuffix}" };
            await _dbContext.Locations.AddAsync(location);
            await _dbContext.SaveChangesAsync();

            var locationName = new LocationName { Name = location.LocationName };
            var serializedLocationName = JsonHelper.Serialize(locationName);
            var requestContent = new StringContent(serializedLocationName, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync("/api/location/add", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task RemoveLocation_ShouldReturnOk_WhenLocationIsRemovedSuccessfully()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var location = new Location { LocationName = $"Location_{uniqueSuffix}" };
            await _dbContext.Locations.AddAsync(location);
            await _dbContext.SaveChangesAsync();

            var locationName = new LocationName { Name = location.LocationName };
            var serializedLocationName = JsonHelper.Serialize(locationName);
            var requestContent = new StringContent(serializedLocationName, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/location/remove")
            {
                Content = requestContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task RemoveLocation_ShouldReturnNotFound_WhenLocationDoesNotExist()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var locationName = new LocationName { Name = $"NonExistentLocation_{uniqueSuffix}" };
            var serializedLocationName = JsonHelper.Serialize(locationName);
            var requestContent = new StringContent(serializedLocationName, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/location/remove")
            {
                Content = requestContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetDesks_ShouldReturnOk_WhenLocationExists()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var location = new Location { LocationName = $"Location_{uniqueSuffix}" };
            var desk = new Desk { DeskName = $"Desk_{uniqueSuffix}", IsEnabled = true, Location = location };
            await _dbContext.Locations.AddAsync(location);
            await _dbContext.Desks.AddAsync(desk);
            await _dbContext.SaveChangesAsync();

            var token = _sessionTokenManager.CreateToken("testuser", "Employee");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"/api/location/getdesks/{location.LocationName}");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetDesks_ShouldReturnNotFound_WhenLocationDoesNotExist()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var token = _sessionTokenManager.CreateToken("testuser", "Employee");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync($"/api/location/getdesks/NonExistentLocation_{uniqueSuffix}");

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetAllNames_ShouldReturnOk()
        {
            // Arrange
            var token = _sessionTokenManager.CreateToken("testuser", "Employee");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync("/api/location/getallnames");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOk_WhenUserIsAdmin()
        {
            // Arrange
            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.GetAsync("/api/location/getall");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
