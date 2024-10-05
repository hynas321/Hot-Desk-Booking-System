namespace WebApi.Services;

public interface IDailyTaskService
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
}
