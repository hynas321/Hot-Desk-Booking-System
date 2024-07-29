using Dotnet.Server.Managers;
using Dotnet.Server.Http;
using Dotnet.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Server.Configuration;
using Dotnet.Server.Services;

namespace Dotnet.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILogger<LocationController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly ILocationService _locationService;
    private readonly ISessionTokenManager _tokenManager;

    public LocationController(
        ILogger<LocationController> logger,
        IConfiguration configuration,
        IUserService userService,
        ILocationService locationService,
        ISessionTokenManager tokenManager
    )
    {
        _logger = logger;
        _configuration = configuration;
        _userService = userService;
        _locationService = locationService;
        _tokenManager = tokenManager;
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromHeader] string token, [FromBody] LocationName locationName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Add: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (token != _configuration[Config.GlobalAdminToken])
            {
                string? username = _tokenManager.GetUsername(token);

                if (username == null)
                {
                    _logger.LogInformation("Add: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                User? user = await _userService.GetUserAsync(username, cancellationToken);

                if (user == null || user.IsAdmin == false)
                {
                    _logger.LogError("Add: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            Location? location = await _locationService.GetLocationAsync(locationName.Name, cancellationToken);

            if (location != null)
            {
                _logger.LogInformation("Add: Status 409, Conflict");
                return StatusCode(StatusCodes.Status409Conflict);
            }

            location = new Location()
            {
                LocationName = locationName.Name,
                Desks = new List<Desk>()
            };

            await _locationService.AddLocationAsync(location, cancellationToken);

            _logger.LogInformation("Add: Status 201, Created");
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("Remove")]
    public async Task<IActionResult> Remove([FromHeader] string token, [FromBody] LocationName locationName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Remove: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (token != _configuration[Config.GlobalAdminToken])
            {
                string? username = _tokenManager.GetUsername(token);

                if (username == null)
                {
                    _logger.LogInformation("Remove: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                User? user = await _userService.GetUserAsync(username, cancellationToken);

                if (user == null || user.IsAdmin == false)
                {
                    _logger.LogError("Remove: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            Location? location = await _locationService.GetLocationAsync(locationName.Name, cancellationToken);

            if (location == null)
            {
                _logger.LogInformation("Remove: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            bool isLocationRemoved = await _locationService.RemoveLocationAsync(location.LocationName, cancellationToken);

            if (!isLocationRemoved)
            {
                _logger.LogInformation("Remove: Status 500, Internal server error");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            _logger.LogInformation("Remove: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetDesks/{locationName}")]
    public async Task<IActionResult> GetDesks([FromHeader] string token, [FromRoute] string locationName, CancellationToken cancellationToken = default)
    {
        try
        {
            Location? location = await _locationService.GetLocationAsync(locationName, cancellationToken);

            if (location == null)
            {
                _logger.LogInformation("GetDesks: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            List<DeskDTO> clientsideDesks = new List<DeskDTO>();
            string? username = null;
            User? user = null;


            if (token != _configuration[Config.GlobalAdminToken])
            {
                username = _tokenManager.GetUsername(token);

                if (username == null)
                {
                    _logger.LogInformation("GetDesks: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                user = await _userService.GetUserAsync(username, cancellationToken);

                if (user == null)
                {
                    _logger.LogError("GetDesks: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            // Map desks to DeskDTOs
            foreach (var desk in location.Desks)
            {
                DeskDTO deskDTO = new DeskDTO
                {
                    DeskName = desk.DeskName,
                    IsEnabled = desk.IsEnabled,
                    Username = (user?.IsAdmin == true || desk.Bookings.Any(b => b.UserId == username)) ? desk.Bookings.FirstOrDefault(b => b.UserId == username)?.User?.UserName ?? "-" : "-",
                    StartTime = desk.Bookings.FirstOrDefault(b => b.UserId == username)?.StartTime?.ToString("dd-MM-yyyy"),
                    EndTime = desk.Bookings.FirstOrDefault(b => b.UserId == username)?.EndTime?.ToString("dd-MM-yyyy")
                };

                clientsideDesks.Add(deskDTO);
            }

            _logger.LogInformation("GetDesks: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, clientsideDesks);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetAllNames")]
    public async Task<IActionResult> GetAllNames(CancellationToken cancellationToken = default)
    {
        try
        {
            List<Location> locations = await _locationService.GetAllLocationsAsync(cancellationToken);

            List<ClientsideLocation> clientsideLocations = locations.Select(location => new ClientsideLocation
            {
                LocationName = location.LocationName,
                TotalDeskCount = location.Desks.Count,
                AvailableDeskCount = location.Desks.Count(d => !d.Bookings.Any() && d.IsEnabled)
            }).ToList();

            _logger.LogInformation("GetAllNames: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, clientsideLocations);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromHeader] string globalAdminToken, CancellationToken cancellationToken = default)
    {
        try
        {
            if (globalAdminToken != _configuration[Config.GlobalAdminToken])
            {
                _logger.LogError("GetAll: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            List<Location> locations = await _locationService.GetAllLocationsAsync(cancellationToken);

            _logger.LogInformation("GetAll: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(locations));

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
