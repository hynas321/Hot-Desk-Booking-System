using Dotnet.Server.Http;

namespace Dotnet.Server.Repositories;

public class BookingRepository : IBookingRepository
{
    #nullable disable warnings
    private readonly ApplicationDbContext dbContext;
    private readonly IDeskRepository deskRepository;

    public BookingRepository(ApplicationDbContext dbContext, IDeskRepository deskRepository)
    {
        this.dbContext = dbContext;
        this.deskRepository = deskRepository;
    }

    public async Task<ClientsideDesk?> BookDeskAsync(string username, BookingInformation bookingInformation, CancellationToken cancellationToken)
    {
        Location? location = dbContext?.Locations?.FirstOrDefault(l => l.LocationName == bookingInformation.LocationName);

        if (location != null)
        {
            Desk? desk = location.Desks.FirstOrDefault(d => d.DeskName == bookingInformation.DeskName);

            List<Desk> desks = await deskRepository.GetAllDesksAsync(cancellationToken);
            bool existingAnyUserBookings = desks.Any(d => d.Booking?.Username == username);

            if (desk != null && desk?.Booking?.Username == null && !existingAnyUserBookings && desk.IsEnabled)
            {
                DateTime utcNow = DateTime.UtcNow;
                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, localTimeZone);

                desk.Booking.Username = username;
                desk.Booking.StartTime = localTime;
                desk.Booking.EndTime = localTime.AddDays(bookingInformation.Days - 1);

                dbContext?.Desks.Update(desk);
                await dbContext?.SaveChangesAsync(cancellationToken);

                string bookingStartTimeString = desk.Booking.StartTime.Value.ToString("dd-MM-yyyy");
                string bookingEndTimeString = desk.Booking.EndTime.Value.ToString("dd-MM-yyyy");

                ClientsideDesk clientsideDesk = new ClientsideDesk()
                {
                    DeskName = desk.DeskName,
                    IsEnabled = desk.IsEnabled,
                    Username = desk.Booking.Username,
                    StartTime = bookingStartTimeString,
                    EndTime = bookingEndTimeString
                };

                return clientsideDesk;
            }
        }

        return null;
    }

    public async Task<ClientsideDesk?> UnBookDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken)
    {
        Location? location = dbContext?.Locations?.FirstOrDefault(l => l.LocationName == deskInformation.LocationName);

        if (location != null)
        {
            Desk? desk = location.Desks.FirstOrDefault(d => d.DeskName == deskInformation.DeskName);

            if (desk != null && desk?.Booking?.Username != null)
            {
                desk.Booking.Username = null;
                desk.Booking.StartTime = null;
                desk.Booking.EndTime = null;

                dbContext?.Desks.Update(desk);
                await dbContext?.SaveChangesAsync(cancellationToken);

                ClientsideDesk clientsideDesk = new ClientsideDesk()
                {
                    DeskName = desk.DeskName,
                    Username = null,
                    IsEnabled = desk.IsEnabled,
                    StartTime =
                        desk.Booking.StartTime.HasValue ?
                        desk.Booking.StartTime.Value.ToString("dd-MM-yyyy")
                        : null,
                    EndTime =
                        desk.Booking.EndTime.HasValue ?
                        desk.Booking.EndTime.Value.ToString("dd-MM-yyyy")
                        : null
                };

                return clientsideDesk;
            }
        }

        return null;
    }
}
