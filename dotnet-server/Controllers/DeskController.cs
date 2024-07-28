using Dotnet.Server.Managers;
using Dotnet.Server.Repositories;
using Dotnet.Server.Http;
using Dotnet.Server.Configuration;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Server.Helpers;

namespace Dotnet.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeskController : ControllerBase
{
    private readonly ILogger<DeskController> logger;
    private readonly IConfiguration configuration;
    private readonly IUserRepository userRepository;
    private readonly IDeskRepository deskRepository;
    private readonly ISessionTokenManager tokenManager;

    public DeskController(
        ILogger<DeskController> logger,
        IConfiguration configuration,
        IUserRepository userRepository,
        IDeskRepository deskRepository,
        ISessionTokenManager tokenManager
    )
    {
        this.logger = logger;
        this.configuration = configuration;
        this.userRepository = userRepository;
        this.deskRepository = deskRepository;
        this.tokenManager = tokenManager;
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromHeader] string token, [FromBody] DeskInformation deskInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Add: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (token != configuration[Config.GlobalAdminToken])
            {
                string? username = tokenManager.GetUsername(token);

                if (username == null)
                {
                    logger.LogError("Add: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                User? user = await userRepository.GetUserAsync(username, cancellationToken);

                if (user == null || user.IsAdmin == false)
                {
                    logger.LogError("Add: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            Desk desk = await deskRepository.GetDeskAsync(deskInfo, cancellationToken);

            if (desk == null)
            {
                logger.LogInformation("Add: Status 409, Conflict");
                return StatusCode(StatusCodes.Status409Conflict);
            }

            bool deskAdded = await deskRepository.AddDeskAsync(deskInfo, cancellationToken);

            if (!deskAdded)
            {
                logger.LogInformation("Add: Status 500, Internal server error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            logger.LogInformation("Add: Status 201, Created");
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
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
                logger.LogError("Remove: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (token != configuration[Config.GlobalAdminToken])
            {
                string? username = tokenManager.GetUsername(token);

                if (username == null)
                {
                    logger.LogInformation("Remove: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                User? user = await userRepository.GetUserAsync(username, cancellationToken);

                if (user == null || user.IsAdmin == false)
                {
                    logger.LogError("Remove: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            Desk desk = await deskRepository.GetDeskAsync(deskInfo, cancellationToken);

            if (desk == null)
            {
                logger.LogInformation("Remove: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            bool isDeskRemoved = await deskRepository.RemoveDeskAsync(deskInfo, cancellationToken);

            if (!isDeskRemoved)
            {
                logger.LogInformation("Remove: 500, Internal server error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            logger.LogInformation("Remove: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
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
                logger.LogError("SetDeskAvailability: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (token != configuration[Config.GlobalAdminToken])
            {
                string? username = tokenManager.GetUsername(token);

                if (username == null)
                {
                    logger.LogError("SetDeskAvailability: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                User? user = await userRepository.GetUserAsync(username, cancellationToken);

                if (user == null || user.IsAdmin == false)
                {
                    logger.LogError("SetDeskAvailability: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            DeskInformation info = new DeskInformation()
            {
                DeskName = deskAvailabilityInfo.DeskName,
                LocationName = deskAvailabilityInfo.LocationName
            };

            Desk desk = await deskRepository.GetDeskAsync(info, cancellationToken);

            if (desk == null)
            {
                logger.LogInformation("SetDeskAvailability: Status 404 Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            ClientsideDesk? clientSideDesk = await deskRepository.SetDeskAvailabilityAsync(info, deskAvailabilityInfo.IsEnabled, cancellationToken);

            if (clientSideDesk == null)
            {
                logger.LogInformation("SetDeskAvailability: Status 500, Internal server error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            logger.LogInformation("SetDeskAvailability: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(clientSideDesk));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}