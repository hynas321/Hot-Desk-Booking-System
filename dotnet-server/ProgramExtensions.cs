using Dotnet.Server.Managers;
using Dotnet.Server.Repositories;
using Dotnet.Server.Services;

public static class ProgramExtensions
{
    public static IServiceCollection AddManagers(this IServiceCollection services)
    {
        services.AddScoped<ISessionTokenManager, SessionTokenManager>();
        services.AddScoped<IHashManager, HashManager>();
        return services;
    }

    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IDeskRepository, DeskRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IDeskService, DeskService>();
        services.AddScoped<IBookingService, BookingService>();
        return services;
    }

    public static IServiceCollection AddHostedServices(this IServiceCollection services)
    {
        services.AddHostedService<DailyTaskService>();
        return services;
    }
}
