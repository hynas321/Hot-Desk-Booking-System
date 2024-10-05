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
    public class BookingControllerTests : IClassFixture<DockerWebApplicationFactoryFixture>
    {
        private readonly HttpClient _client;
        private readonly IServiceScope _scope;
        private readonly ISessionTokenManager _sessionTokenManager;

        public BookingControllerTests(DockerWebApplicationFactoryFixture factory)
        {
            _client = factory.CreateClient();
            _scope = factory.Services.CreateScope();

            _sessionTokenManager = _scope.ServiceProvider.GetRequiredService<ISessionTokenManager>();
        }

        [Fact]
        public async Task BookDesk_ShouldReturnOk_WhenBookingIsSuccessful()
        {
            // Arrange
            var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var location = new Location { LocationName = $"Location_{uniqueSuffix}" };

            await dbContext.Locations.AddAsync(location);

            var desk = new Desk { DeskName = $"Desk_{uniqueSuffix}", IsEnabled = true, Location = location };
            await dbContext.Desks.AddAsync(desk);

            var user = new User { Username = $"testuser_{uniqueSuffix}", Password = "password123" };
            await dbContext.Users.AddAsync(user);

            await dbContext.SaveChangesAsync();

            var bookingInfo = new BookingInformation { DeskName = desk.DeskName, LocationName = location.LocationName, Days = 1 };
            var serializedBookingInfo = JsonHelper.Serialize(bookingInfo);
            var requestContent = new StringContent(serializedBookingInfo, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken(user.Username, "Employee");

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
            var dbContext = _scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

            var uniqueSuffix = DateTime.UtcNow.Ticks.ToString();
            var location = new Location { LocationName = $"Location_{uniqueSuffix}" };
            await dbContext.Locations.AddAsync(location);

            var desk = new Desk { DeskName = $"Desk_{uniqueSuffix}", IsEnabled = true, Location = location };
            await dbContext.Desks.AddAsync(desk);

            var user = new User { Username = $"testuser_{uniqueSuffix}", Password = "password123" };
            await dbContext.Users.AddAsync(user);

            var booking = new Booking
            {
                User = user,
                Desk = desk,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddDays(1)
            };
            await dbContext.Bookings.AddAsync(booking);

            await dbContext.SaveChangesAsync();

            var deskInfo = new DeskInformation { DeskName = desk.DeskName, LocationName = location.LocationName };
            var serializedDeskInfo = JsonHelper.Serialize(deskInfo);
            var requestContent = new StringContent(serializedDeskInfo, Encoding.UTF8, "application/json");

            var token = _sessionTokenManager.CreateToken(user.Username, "Employee");

            // Act
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _client.PutAsync("/api/booking/unbook", requestContent);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
