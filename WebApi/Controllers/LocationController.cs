using WebApi.Http;
using WebApi.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using WebApi.Models.Constants;
using WebApi.Managers.Abstractions;
using WebApi.Models;
using WebApi.Services.Abstractions;

namespace WebApi.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILogger<LocationController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly ILocationService _locationService;
    private readonly ISessionTokenManager _sessionTokenManager;

    public LocationController(
        ILogger<LocationController> logger,
        IConfiguration configuration,
        IUserService userService,
        ILocationService locationService,
        ISessionTokenManager sessionTokenManager
    )
    {
        _logger = logger;
        _configuration = configuration;
        _userService = userService;
        _locationService = locationService;
        _sessionTokenManager = sessionTokenManager;
    }

    [Authorize(Roles = "Admin")]
    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] LocationName locationName, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        if (await _locationService.GetLocationAsync(locationName.Name, cancellationToken) != null)
        {
            return Conflict();
        }

        var location = new Location
        {
            LocationName = locationName.Name,
            Desks = new List<Desk>()
        };

        await _locationService.AddLocationAsync(location, cancellationToken);

        return CreatedAtAction(nameof(Add), new { locationName = location.LocationName });
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpDelete("Remove")]
    public async Task<IActionResult> Remove([FromBody] LocationName locationName, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var location = await _locationService.GetLocationAsync(locationName.Name, cancellationToken);

        if (location == null)
        {
            return NotFound();
        }

        bool isRemoved = await _locationService.RemoveLocationAsync(location.LocationName, cancellationToken);

        if (!isRemoved)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }

    [Authorize]
    [HttpGet("GetDesks/{locationName}")]
    public async Task<IActionResult> GetDesks([FromRoute] string locationName, CancellationToken cancellationToken = default)
    {
        var location = await _locationService.GetLocationAsync(locationName, cancellationToken);

        if (location == null)
        {
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

        return Ok(JsonHelper.Serialize(locationDTO));
    }
}
