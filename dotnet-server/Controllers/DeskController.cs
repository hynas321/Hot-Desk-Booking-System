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
        IUserService userRepository,
        IDeskService deskRepository,
        ISessionTokenManager tokenManager
    )
    {
        _logger = logger;
        _configuration = configuration;
        _userService = userRepository;
        _deskService = deskRepository;
        _tokenManager = tokenManager;
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromHeader] string token, [FromBody] DeskInformation deskInfo, CancellationToken cancellationToken = default)
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
                    _logger.LogError("Add: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                User? user = await _userService.GetUserAsync(username, cancellationToken);

                if (user == null || user.IsAdmin == false)
                {
                    _logger.LogError("Add: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            Desk? desk = await _deskService.GetDeskAsync(deskInfo, cancellationToken);

            if (desk == null)
            {
                _logger.LogInformation("Add: Status 409, Conflict");
                return StatusCode(StatusCodes.Status409Conflict);
            }

            bool deskAdded = await _deskService.AddDeskAsync(deskInfo, cancellationToken);

            if (!deskAdded)
            {
                _logger.LogInformation("Add: Status 500, Internal server error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

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
    public async Task<IActionResult> Remove([FromHeader] string token, [FromBody] DeskInformation deskInfo, CancellationToken cancellationToken = default)
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

            Desk? desk = await _deskService.GetDeskAsync(deskInfo, cancellationToken);

            if (desk == null)
            {
                _logger.LogInformation("Remove: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            bool isDeskRemoved = await _deskService.RemoveDeskAsync(deskInfo, cancellationToken);

            if (!isDeskRemoved)
            {
                _logger.LogInformation("Remove: 500, Internal server error");
                return StatusCode(StatusCodes.Status500InternalServerError);
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

    [HttpPut("SetDeskAvailability")]
    public async Task<IActionResult> SetDeskAvailability([FromHeader] string token, [FromBody] DeskAvailabilityInformation deskAvailabilityInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("SetDeskAvailability: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (token != _configuration[Config.GlobalAdminToken])
            {
                string? username = _tokenManager.GetUsername(token);

                if (username == null)
                {
                    _logger.LogError("SetDeskAvailability: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                User? user = await _userService.GetUserAsync(username, cancellationToken);

                if (user == null || user.IsAdmin == false)
                {
                    _logger.LogError("SetDeskAvailability: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            DeskInformation info = new DeskInformation()
            {
                DeskName = deskAvailabilityInfo.DeskName,
                LocationName = deskAvailabilityInfo.LocationName
            };

            Desk? desk = await _deskService.GetDeskAsync(info, cancellationToken);

            if (desk == null)
            {
                _logger.LogInformation("SetDeskAvailability: Status 404 Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            DeskDTO? clientSideDesk = await _deskService.SetDeskAvailabilityAsync(info, deskAvailabilityInfo.IsEnabled, cancellationToken);

            if (clientSideDesk == null)
            {
                _logger.LogInformation("SetDeskAvailability: Status 500, Internal server error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            _logger.LogInformation("SetDeskAvailability: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(clientSideDesk));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}