using Dotnet.Server.Managers;
using Dotnet.Server.Http;
using Dotnet.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Server.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using dotnet_server.Models.Constants;

namespace Dotnet.Server.Controllers;

[ApiController]
[Authorize]
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

    [Authorize(Roles = "Admin")]
    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] LocationName locationName, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Add: Status 400, Bad Request");
            return BadRequest();
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

    [Authorize(Roles = UserRole.Admin)]
    [HttpDelete("Remove")]
    public async Task<IActionResult> Remove([FromBody] LocationName locationName, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("Remove: Status 400, Bad Request");
            return BadRequest();
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

    [Authorize]
    [HttpGet("GetDesks/{locationName}")]
    public async Task<IActionResult> GetDesks([FromRoute] string locationName, CancellationToken cancellationToken = default)
    {
        var location = await _locationService.GetLocationAsync(locationName, cancellationToken);

        if (location == null)
        {
            _logger.LogInformation("GetDesks: Status 404, Not Found");
            return NotFound();
        }

        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
                Username = relevantBooking?.User?.Username,
                StartTime = relevantBooking?.StartTime?.ToString("dd-MM-yyyy") ?? "-",
                EndTime = relevantBooking?.EndTime?.ToString("dd-MM-yyyy") ?? "-"
            };
        }).ToList();

        _logger.LogInformation("GetDesks: Status 200, OK");
        return Ok(JsonHelper.Serialize(desksDTO));
    }

    [Authorize]
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

    [Authorize(Roles = UserRole.Admin)]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var locations = await _locationService.GetAllLocationsAsync(cancellationToken);

        _logger.LogInformation("GetAll: Status 200, OK");
        return Ok(JsonHelper.Serialize(locations));
    }
}
