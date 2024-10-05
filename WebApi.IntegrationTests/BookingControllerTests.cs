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
    public class BookingControllerTests : IClassFixture<DockerWebApplicationFactoryFixture>
    {
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly ISessionTokenManager _sessionTokenManager;
        private readonly ApplicationDbContext _dbContext;

        public BookingControllerTests(DockerWebApplicationFactoryFixture factory)
        {
            _client = factory.CreateClient();
            _scope = factory.Services.CreateScope();

            _sessionTokenManager = _scope.ServiceProvider.GetRequiredService<ISessionTokenManager>();
            _dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>()
                ?? throw new Exception("DbContext does not exist");
        }

        [Fact]
        public async Task BookDesk_ShouldReturnOk_WhenBookingIsSuccessful()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var location = new Location { LocationName = $"Location_{uniqueSuffix}" };

            await _dbContext.Locations.AddAsync(location);

            var desk = new Desk { DeskName = $"Desk_{uniqueSuffix}", IsEnabled = true, Location = location };
            await _dbContext.Desks.AddAsync(desk);

            var user = new User { Username = $"testuser_{uniqueSuffix}", Password = "password123" };
            await _dbContext.Users.AddAsync(user);

            await _dbContext.SaveChangesAsync();

            var bookingInfo = new BookingInformation { DeskName = desk.DeskName, LocationName = location.LocationName, Days = 1 };
            var serializedBookingInfo = JsonHelper.Serialize(bookingInfo);
            var requestContent = new StringContent(serializedBookingInfo, Encoding.UTF8, "application/json");
            var token = _sessionTokenManager.CreateToken(user.Username, UserRole.User);

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync("/api/booking/book", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task UnbookDesk_ShouldReturnOk_WhenUnbookingIsSuccessful()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var location = new Location { LocationName = $"Location_{uniqueSuffix}" };
            await _dbContext.Locations.AddAsync(location);

            var desk = new Desk { DeskName = $"Desk_{uniqueSuffix}", IsEnabled = true, Location = location };
            await _dbContext.Desks.AddAsync(desk);

            var user = new User { Username = $"testuser_{uniqueSuffix}", Password = "password123" };
            await _dbContext.Users.AddAsync(user);

            var booking = new Booking
            {
                User = user,
                Desk = desk,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(1)
            };

            await _dbContext.Bookings.AddAsync(booking);

            await _dbContext.SaveChangesAsync();

            var deskInfo = new DeskInformation { DeskName = desk.DeskName, LocationName = location.LocationName };
            var serializedDeskInfo = JsonHelper.Serialize(deskInfo);
            var requestContent = new StringContent(serializedDeskInfo, Encoding.UTF8, "application/json");
            var token = _sessionTokenManager.CreateToken(user.Username, UserRole.User);

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync("/api/booking/unbook", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task BookDesk_ShouldReturnConflict_WhenDeskIsAlreadyBooked()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();

            var location = new Location { LocationName = $"Location_{uniqueSuffix}" };
            await _dbContext.Locations.AddAsync(location);

            var desk = new Desk { DeskName = $"Desk_{uniqueSuffix}", IsEnabled = true, Location = location };
            await _dbContext.Desks.AddAsync(desk);

            var user1 = new User { Username = $"testuser1_{uniqueSuffix}", Password = "password123" };
            await _dbContext.Users.AddAsync(user1);

            var user2 = new User { Username = $"testuser2_{uniqueSuffix}", Password = "password123" };
            await _dbContext.Users.AddAsync(user2);

            var booking = new Booking
            {
                User = user1,
                Desk = desk,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(1)
            };
            await _dbContext.Bookings.AddAsync(booking);
            await _dbContext.SaveChangesAsync();

            var bookingInfo = new BookingInformation { DeskName = desk.DeskName, LocationName = location.LocationName, Days = 1 };
            var serializedBookingInfo = JsonHelper.Serialize(bookingInfo);
            var requestContent = new StringContent(serializedBookingInfo, Encoding.UTF8, "application/json");
            var token = _sessionTokenManager.CreateToken(user2.Username, UserRole.User);

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync("/api/booking/book", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
        }

        [Fact]
        public async Task BookDesk_ShouldReturnNotFound_WhenDeskDoesNotExist()
        {
            // Arrange
            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var bookingInfo = new BookingInformation { DeskName = $"NonExistentDesk_{uniqueSuffix}", LocationName = $"Location_{uniqueSuffix}", Days = 1 };
            var serializedBookingInfo = JsonHelper.Serialize(bookingInfo);
            var requestContent = new StringContent(serializedBookingInfo, Encoding.UTF8, "application/json");
            var token = _sessionTokenManager.CreateToken("testuser", UserRole.User);

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync("/api/booking/book", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task BookDesk_ShouldReturnBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            var bookingInfo = new BookingInformation();
            var serializedBookingInfo = JsonHelper.Serialize(bookingInfo);
            var requestContent = new StringContent(serializedBookingInfo, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("testuser", UserRole.User);

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync("/api/booking/book", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task UnbookDesk_ShouldReturnBadRequest_WhenDataIsInvalid()
        {
            // Arrange
            var deskInfo = new DeskInformation();
            var serializedDeskInfo = JsonHelper.Serialize(deskInfo);
            var requestContent = new StringContent(serializedDeskInfo, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken("testuser", UserRole.User);

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync("/api/booking/unbook", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }
    }
}
