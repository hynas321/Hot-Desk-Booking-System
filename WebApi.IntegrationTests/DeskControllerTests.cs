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
    public class DeskControllerTests : IClassFixture<DockerWebApplicationFactoryFixture>
    {
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly ISessionTokenManager _sessionTokenManager;

        public DeskControllerTests(DockerWebApplicationFactoryFixture factory)
        {
            _client = factory.CreateClient();
            _scope = factory.Services.CreateScope();

            _sessionTokenManager = _scope.ServiceProvider.GetRequiredService<ISessionTokenManager>();
        }

        [Fact]
        public async Task AddDesk_ShouldReturnCreated_WhenDeskIsAddedSuccessfully()
        {
            // Arrange
            var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();

            var location = new Location { LocationName = $"Location_{uniqueSuffix}" };
            await dbContext.Locations.AddAsync(location);
            await dbContext.SaveChangesAsync();

            var deskInfo = new DeskInformation { DeskName = $"Desk_{uniqueSuffix}", LocationName = location.LocationName };
            var serializedDeskInfo = JsonHelper.Serialize(deskInfo);
            var requestContent = new StringContent(serializedDeskInfo, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync("/api/desk/add", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task AddDesk_ShouldReturnConflict_WhenDeskAlreadyExists()
        {
            // Arrange
            var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();

            var location = new Location { LocationName = $"Location_{uniqueSuffix}" };
            await dbContext.Locations.AddAsync(location);

            var desk = new Desk { DeskName = $"Desk_{uniqueSuffix}", IsEnabled = true, Location = location };
            await dbContext.Desks.AddAsync(desk);
            await dbContext.SaveChangesAsync();

            var deskInfo = new DeskInformation { DeskName = desk.DeskName, LocationName = location.LocationName };
            var serializedDeskInfo = JsonHelper.Serialize(deskInfo);
            var requestContent = new StringContent(serializedDeskInfo, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync("/api/desk/add", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task RemoveDesk_ShouldReturnOk_WhenDeskIsRemovedSuccessfully()
        {
            // Arrange
            var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();

            var location = new Location { LocationName = $"Location_{uniqueSuffix}" };
            await dbContext.Locations.AddAsync(location);

            var desk = new Desk { DeskName = $"Desk_{uniqueSuffix}", IsEnabled = true, Location = location };
            await dbContext.Desks.AddAsync(desk);
            await dbContext.SaveChangesAsync();

            var deskInfo = new DeskInformation { DeskName = desk.DeskName, LocationName = location.LocationName };
            var serializedDeskInfo = JsonHelper.Serialize(deskInfo);
            var requestContent = new StringContent(serializedDeskInfo, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/desk/remove")
            {
                Content = requestContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task RemoveDesk_ShouldReturnNotFound_WhenDeskDoesNotExist()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var deskInfo = new DeskInformation { DeskName = $"NonExistentDesk_{uniqueSuffix}", LocationName = $"Location_{uniqueSuffix}" };
            var serializedDeskInfo = JsonHelper.Serialize(deskInfo);
            var requestContent = new StringContent(serializedDeskInfo, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            var request = new HttpRequestMessage(HttpMethod.Delete, "/api/desk/remove")
            {
                Content = requestContent
            };
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


        [Fact]
        public async Task SetDeskAvailability_ShouldReturnOk_WhenDeskAvailabilityIsUpdatedSuccessfully()
        {
            // Arrange
            var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();

            var location = new Location { LocationName = $"Location_{uniqueSuffix}" };
            await dbContext.Locations.AddAsync(location);

            var desk = new Desk { DeskName = $"Desk_{uniqueSuffix}", IsEnabled = false, Location = location };
            await dbContext.Desks.AddAsync(desk);
            await dbContext.SaveChangesAsync();

            var deskAvailabilityInfo = new DeskAvailabilityInformation
            {
                DeskName = desk.DeskName,
                LocationName = location.LocationName,
                IsEnabled = true
            };
            var serializedDeskAvailabilityInfo = JsonHelper.Serialize(deskAvailabilityInfo);
            var requestContent = new StringContent(serializedDeskAvailabilityInfo, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync("/api/desk/setdeskavailability", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task SetDeskAvailability_ShouldReturnNotFound_WhenDeskDoesNotExist()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var deskAvailabilityInfo = new DeskAvailabilityInformation
            {
                DeskName = $"NonExistentDesk_{uniqueSuffix}",
                LocationName = $"Location_{uniqueSuffix}",
                IsEnabled = true
            };
            var serializedDeskAvailabilityInfo = JsonHelper.Serialize(deskAvailabilityInfo);
            var requestContent = new StringContent(serializedDeskAvailabilityInfo, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync("/api/desk/setdeskavailability", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AddDesk_ShouldReturnBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            var deskInfo = new DeskInformation();
            var serializedDeskInfo = JsonHelper.Serialize(deskInfo);
            var requestContent = new StringContent(serializedDeskInfo, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("adminuser", "Admin");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PostAsync("/api/desk/add", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
