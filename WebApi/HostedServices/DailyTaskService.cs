using WebApi.Http;
using WebApi.Repositories;
using WebApi.Services.Abstractions;

namespace WebApi.Services;

public class DailyTaskService : IHostedService
{
    private readonly ILogger<DailyTaskService> logger;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private Timer? timer;
    private static bool activatedAfterServerStart = false;

    public DailyTaskService(
        ILogger<DailyTaskService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        if (!activatedAfterServerStart)
        {
            DoWork(null);
            activatedAfterServerStart = true;
        }

        var utcNow = DateTime.UtcNow;
        var nextRun = utcNow.Date.AddDays(1).AddHours(0);
        var timeUntilNextRun = nextRun - utcNow;

        timer = new Timer(DoWork, null, timeUntilNextRun, TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        logger.LogInformation("Starting daily task execution.");

        try
        {
            using (var scope = serviceScopeFactory.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
                var deskRepository = serviceProvider.GetRequiredService<IDeskService>();
                var bookingService = serviceProvider.GetRequiredService<IBookingService>();

                var desks = deskRepository.GetAllDesks();
                logger.LogInformation("Checking for obsolete bookings...");

                int obsoleteBookingsCount = 0;

                foreach (var desk in desks)
                {
                    var obsoleteBookings = desk.Bookings.Where(b => b.EndTime < DateTime.UtcNow).ToList();

                    foreach (var booking in obsoleteBookings)
                    {
                        DeskInformation deskInformation = new DeskInformation()
                        {
                            DeskName = desk.DeskName,
                            LocationName = desk.Location.LocationName
                        };

                        bookingService.RemoveBookingAsync(deskInformation, default).Wait();
                        obsoleteBookingsCount++;
                    }
                }

                logger.LogInformation($"Found {obsoleteBookingsCount} obsolete bookings");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred during the execution of the daily task.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer?.Dispose();
        return Task.CompletedTask;
    }
}