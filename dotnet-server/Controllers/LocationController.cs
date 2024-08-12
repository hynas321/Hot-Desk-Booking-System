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
        if (!ModelState.IsValid)
        {
            _logger.LogError("Add: Status 400, Bad Request");
            return BadRequest();
        }

        if (!await IsAuthorizedAdminAsync(token, cancellationToken))
        {
            return Unauthorized();
        }

        if (await _locationService.GetLocationAsync(locationName.Name, cancellationToken) != null)
        {
            _logger.LogInformation("Add: Status 409, Conflict");
            return Conflict();
        }

        var location = new Location
        {
            LocationName = locationName.Name,
            Desks = new List<Desk>()
        };

        await _locationService.AddLocationAsync(location, cancellationToken);

        _logger.LogInformation("Add: Status 201, Created");
        return CreatedAtAction(nameof(Add), new { locationName = location.LocationName });
    }

    [HttpDelete("Remove")]
    public async Task<IActionResult> Remove([FromHeader] string token, [FromBody] LocationName locationName, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Remove: Status 400, Bad Request");
            return BadRequest();
        }

        if (!await IsAuthorizedAdminAsync(token, cancellationToken))
        {
            return Unauthorized();
        }

        var location = await _locationService.GetLocationAsync(locationName.Name, cancellationToken);
        if (location == null)
        {
            _logger.LogInformation("Remove: Status 404, Not Found");
            return NotFound();
        }

        bool isRemoved = await _locationService.RemoveLocationAsync(location.LocationName, cancellationToken);
        if (!isRemoved)
        {
            _logger.LogError("Remove: Status 500, Internal Server Error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation("Remove: Status 200, OK");
        return Ok();
    }

    [HttpGet("GetDesks/{locationName}")]
    public async Task<IActionResult> GetDesks([FromHeader] string token, [FromRoute] string locationName, CancellationToken cancellationToken = default)
    {
        var location = await _locationService.GetLocationAsync(locationName, cancellationToken);
        if (location == null)
        {
            _logger.LogInformation("GetDesks: Status 404, Not Found");
            return NotFound();
        }

        var username = token != _configuration[Config.GlobalAdminToken]
            ? await GetUsernameFromTokenAsync(token, cancellationToken)
            : null;

        var user = username != null ? await _userService.GetUserAsync(username, cancellationToken) : null;

        if (user == null && username != null)
        {
            _logger.LogWarning($"GetDesks: Could not find user with username {username}");
        }

        var desksDTO = location.Desks.Select(d =>
        {
            var relevantBooking = d.Bookings.FirstOrDefault();

            return new DeskDTO
            {
                DeskName = d.DeskName,
                IsEnabled = d.IsEnabled,
                Username = relevantBooking?.User.UserName,
                StartTime = relevantBooking?.StartTime?.ToString("dd-MM-yyyy") ?? "-",
                EndTime = relevantBooking?.EndTime?.ToString("dd-MM-yyyy") ?? "-"
            };
        }).ToList();

        _logger.LogInformation("GetDesks: Status 200, OK");
        return Ok(JsonHelper.Serialize(desksDTO));
    }

    [HttpGet("GetAllNames")]
    public async Task<IActionResult> GetAllNames(CancellationToken cancellationToken = default)
    {
        var locations = await _locationService.GetAllLocationsAsync(cancellationToken);

        var locationDTO = locations.Select(loc => new LocationDTO
        {
            LocationName = loc.LocationName,
            TotalDeskCount = loc.Desks.Count,
            AvailableDeskCount = loc.Desks.Count(d => !d.Bookings.Any() && d.IsEnabled)
        }).ToList();

        _logger.LogInformation("GetAllNames: Status 200, OK");
        return Ok(JsonHelper.Serialize(locationDTO));
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromHeader] string globalAdminToken, CancellationToken cancellationToken = default)
    {
        if (globalAdminToken != _configuration[Config.GlobalAdminToken])
        {
            _logger.LogError("GetAll: Status 401, Unauthorized");
            return Unauthorized();
        }

        var locations = await _locationService.GetAllLocationsAsync(cancellationToken);

        _logger.LogInformation("GetAll: Status 200, OK");
        return Ok(JsonHelper.Serialize(locations));
    }

    private async Task<bool> IsAuthorizedAdminAsync(string token, CancellationToken cancellationToken)
    {
        if (token != _configuration[Config.GlobalAdminToken])
        {
            var username = await GetUsernameFromTokenAsync(token, cancellationToken);
            if (username == null)
            {
                return false;
            }

            var user = await _userService.GetUserAsync(username, cancellationToken);
            return user?.IsAdmin == true;
        }
        return true;
    }

    private async Task<string?> GetUsernameFromTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await Task.FromResult(_tokenManager.GetUsername(token));
    }
}
