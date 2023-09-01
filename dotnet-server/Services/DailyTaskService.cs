using Dotnet.Server.Database;

namespace Dotnet.Server.Services;

public class DailyTaskService : IHostedService, IDisposable
{
    #nullable disable warnings
    private readonly ILogger<DailyTaskService> logger;
    private readonly IServiceScopeFactory serviceScopeFactory;
    private Timer? timer;

    public DailyTaskService(
        ILogger<DailyTaskService> logger,
        IServiceScopeFactory serviceScopeFactory)
    {
        this.logger = logger;
        this.serviceScopeFactory = serviceScopeFactory;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        DateTime utcNow = DateTime.UtcNow.AddDays(0);
        TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
        DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, localTimeZone);
        DateTime nextRun = utcNow.Date.AddDays(1);
        TimeSpan timeUntilNextRun = nextRun - utcNow;

        timer = new Timer(DoWork, null, timeUntilNextRun, TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        using (var scope = serviceScopeFactory.CreateScope())
        {
            var serviceProvider = scope.ServiceProvider;
            var dbContext = serviceProvider.GetRequiredService<ApplicationDbContext>();
            var deskRepository = scope.ServiceProvider.GetRequiredService<DeskRepository>();

            List<Desk> desks = deskRepository.GetAllDesks();
            logger.LogInformation("Checking for obsolete bookings...");

            for (int i = 0; i < desks.Count; i++)
            {
                DateTime? bookingEnd = desks[i].Booking?.EndTime;

                DateTime utcNow = DateTime.UtcNow.AddDays(0);
                TimeZoneInfo localTimeZone = TimeZoneInfo.Local;
                DateTime localTime = TimeZoneInfo.ConvertTimeFromUtc(utcNow, localTimeZone);

                if (bookingEnd.HasValue)
                {
                    TimeSpan difference = localTime - bookingEnd.Value;

                    if (difference.Days >= 1)
                    {
                        desks[i].Booking.Username = null;
                        desks[i].IsEnabled = true;
                        desks[i].Booking.StartTime = null;
                        desks[i].Booking.EndTime = null;
                        dbContext.Update(desks[i]);
                    }
                }
            }

            dbContext.SaveChanges();
            logger.LogInformation("Procedure complete");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        timer?.Dispose();
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        timer?.Dispose();
    }
}