using Dotnet.Server.Database;

public class DailyTaskService : IHostedService, IDisposable
{
    #nullable disable warnings
    private readonly IServiceProvider? serviceProvider;
    private Timer? timer;

    public DailyTaskService(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        DateTime now = DateTime.UtcNow;
        DateTime nextRun = now.Date.AddDays(1);
        TimeSpan timeUntilNextRun = nextRun - now;

        timer = new Timer(DoWork, null, timeUntilNextRun, TimeSpan.FromDays(1));

        return Task.CompletedTask;
    }

    private void DoWork(object state)
    {
        using (var scope = serviceProvider.CreateScope())
        {
            var deskRepository = scope.ServiceProvider.GetRequiredService<DeskRepository>();

            List<Desk> desks = deskRepository.GetAllDesks();

            for (int i = 0; i < desks.Count; i++)
            {
                DateTime? bookingEnd = desks[i].Booking?.EndTime;
                DateTime now = DateTime.UtcNow;

                if (bookingEnd.HasValue)
                {
                    TimeSpan difference = now - bookingEnd.Value;

                    if (difference.TotalDays >= 1)
                    {
                        desks[i].Booking.Username = null;
                        desks[i].Booking.StartTime = null;
                        desks[i].Booking.EndTime = null;
                    }
                }
            }
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