using WebApi.Http;
using WebApi.Models;
using WebApi.Repositories.Abstractions;

namespace WebApi.Services.Abstractions;

public class BookingService : IBookingService
{
    private readonly IBookingRepository bookingRepository;
    private readonly IDeskRepository deskRepository;

    public BookingService(IBookingRepository bookingRepository, IDeskRepository deskRepository)
    {
        this.bookingRepository = bookingRepository;
        this.deskRepository = deskRepository;
    }

    public async Task<DeskDTO?> AddBookingAsync(User user, BookingInformation bookingInformation, CancellationToken cancellationToken)
    {
        var deskInformation = new DeskInformation()
        {
            DeskName = bookingInformation.DeskName,
            LocationName = bookingInformation.LocationName
        };

        var desk = await deskRepository.GetDeskAsync(deskInformation, cancellationToken);

        if (desk == null || desk.Bookings.Any() || user.Bookings.Any() || !desk.IsEnabled)
        {
            return null;
        }

        var utcNow = DateTime.UtcNow;
        var localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, TimeZoneInfo.Local);
        var startTime = localTime;
        var endTime = localTime.AddDays(bookingInformation.Days - 1);

        Booking newBooking = new Booking
        {
            StartTime = startTime,
            EndTime = endTime,
            User = user,
            Desk = desk
        };

        await bookingRepository.AddBookingAsync(newBooking, cancellationToken);

        return new DeskDTO
        {
            DeskName = desk.DeskName,
            IsEnabled = desk.IsEnabled,
            Username = user.Username,
            StartTime = startTime.ToString("dd-MM-yyyy"),
            EndTime = endTime.ToString("dd-MM-yyyy")
        };
    }

    public async Task<DeskDTO?> RemoveBookingAsync(DeskInformation deskInformation, CancellationToken cancellationToken)
    {
        var desk = await deskRepository.GetDeskAsync(deskInformation, cancellationToken);

        if (desk == null || !desk.Bookings.Any())
        {
            return null;
        }

        var booking = desk.Bookings.First();
        await bookingRepository.RemoveBookingAsync(booking, cancellationToken);

        return new DeskDTO
        {
            DeskName = desk.DeskName,
            IsEnabled = desk.IsEnabled,
            Username = null,
            StartTime = null,
            EndTime = null
        };
    }

    public async Task<Booking?> GetBookingAsync(DeskInformation deskInformation, CancellationToken cancellationToken)
    {
        var desk = await deskRepository.GetDeskAsync(deskInformation, cancellationToken);

        if (desk == null || !desk.Bookings.Any())
        {
            return null;
        }

        return await bookingRepository.GetBookingAsync(deskInformation, cancellationToken);
    }
}