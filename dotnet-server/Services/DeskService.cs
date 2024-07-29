using Dotnet.Server.Http;
using Dotnet.Server.Repositories;

namespace Dotnet.Server.Services;

public class DeskService : IDeskService
{
    private readonly IDeskRepository deskRepository;
    private readonly ILocationRepository locationRepository;

    public DeskService(IDeskRepository deskRepository, ILocationRepository locationRepository)
    {
        this.deskRepository = deskRepository;
        this.locationRepository = locationRepository;
    }

    public async Task<bool> AddDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken)
    {
        var location = await locationRepository.GetLocationAsync(deskInformation.LocationName, cancellationToken);

        if (location == null)
        {
            return false;
        }

        Desk desk = new Desk
        {
            DeskName = deskInformation.DeskName,
            IsEnabled = true,
            Location = location
        };

        await deskRepository.AddDeskAsync(desk, cancellationToken);
        return true;
    }

    public async Task<bool> RemoveDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken)
    {
        var desk = await deskRepository.GetDeskAsync(deskInformation, cancellationToken);

        if (desk == null || desk.Bookings.Any())
        {
            return false;
        }

        await deskRepository.RemoveDeskAsync(desk, cancellationToken);
        return true;
    }

    public async Task<DeskDTO?> SetDeskAvailabilityAsync(DeskInformation deskInformation, bool isEnabled, CancellationToken cancellationToken)
    {
        var desk = await deskRepository.GetDeskAsync(deskInformation, cancellationToken);

        if (desk == null || desk.Bookings.Any())
        {
            return null;
        }

        desk.IsEnabled = isEnabled;
        await deskRepository.UpdateDeskAsync(desk, cancellationToken);

        return new DeskDTO
        {
            DeskName = desk.DeskName,
            IsEnabled = desk.IsEnabled,
            Username = null,
            StartTime = null,
            EndTime = null
        };
    }

    public async Task<Desk?> GetDeskAsync(DeskInformation deskInformation, CancellationToken cancellationToken)
    {
        return await deskRepository.GetDeskAsync(deskInformation, cancellationToken);
    }

    public async Task<List<Desk>> GetAllDesksAsync(CancellationToken cancellationToken)
    {
        return await deskRepository.GetAllDesksAsync(cancellationToken);
    }

    public List<Desk> GetAllDesks()
    {
        return deskRepository.GetAllDesks();
    }
}