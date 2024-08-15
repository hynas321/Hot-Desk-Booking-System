using Dotnet.Server.Managers;
using Dotnet.Server.Http;
using Dotnet.Server.Configuration;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Server.Helpers;
using Dotnet.Server.Services;

namespace Dotnet.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeskController : ControllerBase
{
    private readonly ILogger<DeskController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly IDeskService _deskService;
    private readonly ISessionTokenManager _tokenManager;

    public DeskController(
        ILogger<DeskController> logger,
        IConfiguration configuration,
        IUserService userService,
        IDeskService deskService,
        ISessionTokenManager tokenManager
    )
    {
        _logger = logger;
        _configuration = configuration;
        _userService = userService;
        _deskService = deskService;
        _tokenManager = tokenManager;
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromHeader] string token, [FromBody] DeskInformation deskInfo, CancellationToken cancellationToken = default)
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

        var existingDesk = await _deskService.GetDeskAsync(deskInfo, cancellationToken);
        if (existingDesk != null)
        {
            _logger.LogInformation("Add: Status 409, Conflict");
            return Conflict();
        }

        bool deskAdded = await _deskService.AddDeskAsync(deskInfo, cancellationToken);
        if (!deskAdded)
        {
            _logger.LogError("Add: Status 500, Internal Server Error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation("Add: Status 201, Created");
        return CreatedAtAction(nameof(Add), new { deskInfo.DeskName, deskInfo.LocationName });
    }

    [HttpDelete("Remove")]
    public async Task<IActionResult> Remove([FromHeader] string token, [FromBody] DeskInformation deskInfo, CancellationToken cancellationToken = default)
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

        var desk = await _deskService.GetDeskAsync(deskInfo, cancellationToken);
        if (desk == null)
        {
            _logger.LogInformation("Remove: Status 404, Not Found");
            return NotFound();
        }

        bool isRemoved = await _deskService.RemoveDeskAsync(deskInfo, cancellationToken);
        if (!isRemoved)
        {
            _logger.LogError("Remove: Status 500, Internal Server Error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation("Remove: Status 200, OK");
        return Ok();
    }

    [HttpPut("SetDeskAvailability")]
    public async Task<IActionResult> SetDeskAvailability([FromHeader] string token, [FromBody] DeskAvailabilityInformation deskAvailabilityInfo, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            _logger.LogError("SetDeskAvailability: Status 400, Bad Request");
            return BadRequest();
        }

        if (!await IsAuthorizedAdminAsync(token, cancellationToken))
        {
            return Unauthorized();
        }

        var deskInfo = new DeskInformation
        {
            DeskName = deskAvailabilityInfo.DeskName,
            LocationName = deskAvailabilityInfo.LocationName
        };

        var desk = await _deskService.GetDeskAsync(deskInfo, cancellationToken);
        if (desk == null)
        {
            _logger.LogInformation("SetDeskAvailability: Status 404, Not Found");
            return NotFound();
        }

        var updatedDesk = await _deskService.SetDeskAvailabilityAsync(deskInfo, deskAvailabilityInfo.IsEnabled, cancellationToken);
        if (updatedDesk == null)
        {
            _logger.LogError("SetDeskAvailability: Status 500, Internal Server Error");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        _logger.LogInformation("SetDeskAvailability: Status 200, OK");
        return Ok(JsonHelper.Serialize(updatedDesk));
    }

    private async Task<bool> IsAuthorizedAdminAsync(string token, CancellationToken cancellationToken)
    {
        if (token == _configuration[Config.GlobalAdminToken])
        {
            return true;
        }

        var username = _tokenManager.GetUsername(token);
        if (username == null)
        {
            return false;
        }

        var user = await _userService.GetUserAsync(username, cancellationToken);
        return user?.IsAdmin == true;
    }
}
