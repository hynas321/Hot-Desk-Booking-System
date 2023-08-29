using Dotnet.Server.Authentication;
using Dotnet.Server.Database;
using Dotnet.Server.Http;
using dotnet_server.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILogger<LocationController> logger;
    private readonly IConfiguration configuration;
    private readonly UserRepository userRepository;
    private readonly LocationRepository locationRepository;
    private readonly SessionTokenManager tokenManager;

    public LocationController(
        ILogger<LocationController> logger,
        IConfiguration configuration,
        UserRepository userRepository,
        LocationRepository locationRepository,
        SessionTokenManager tokenManager
    )
    {
        this.logger = logger;
        this.configuration = configuration;
        this.userRepository = userRepository;
        this.locationRepository = locationRepository;
        this.tokenManager = tokenManager;
    }

    [HttpPost("Add")]
    public IActionResult Add([FromHeader] string token, [FromBody] LocationName locationName)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Add: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogInformation("Add: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            User? user = userRepository.GetUser(username);

            if (user == null)
            {
                logger.LogInformation("Add: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            if (user.IsAdmin == false)
            {
                logger.LogError("Add: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            locationRepository.AddLocation(locationName.Name);

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
    public IActionResult Remove([FromHeader] string token, [FromBody] LocationName locationName)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Remove: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogInformation("Remove: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            User? user = userRepository.GetUser(username);

            if (user == null)
            {
                logger.LogInformation("Remove: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            if (user.IsAdmin == false)
            {
                logger.LogError("Remove: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            bool locationRemoved = locationRepository.RemoveLocation(locationName.Name);

            if (!locationRemoved)
            {
                logger.LogInformation("Remove: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
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

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            List<Location> locations = await locationRepository.GetAllLocationsAsync();

            logger.LogInformation("GetAll: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, locations);

        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
