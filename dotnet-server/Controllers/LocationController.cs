using Dotnet.Server.Authentication;
using Dotnet.Server.Database;
using Dotnet.Server.Http;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILogger<LocationController> logger;
    private readonly UserRepository userRepository;
    private readonly LocationRepository locationRepository;
    private readonly SessionTokenManager tokenManager;

    public LocationController(
        ILogger<LocationController> logger,
        UserRepository userRepository,
        LocationRepository locationRepository,
        SessionTokenManager tokenManager
    )
    {
        this.logger = logger;
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

            Location? location = locationRepository.GetLocation(locationName.Name);

            if (location == null)
            {
                logger.LogInformation("Remove: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            if (location.Desks.Count != 0)
            {
                logger.LogInformation("Remove: Status 409, Conflict");
                return StatusCode(StatusCodes.Status409Conflict);
            }

            locationRepository.RemoveLocation(locationName.Name);

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

    [HttpPost("AddDesk")]
    public IActionResult AddDesk([FromHeader] string token, [FromBody] DeskInformation deskInformation)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("AddDesk: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogInformation("AddDesk: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            User? user = userRepository.GetUser(username);

            if (user == null)
            {
                logger.LogInformation("AddDesk: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            if (user.IsAdmin == false)
            {
                logger.LogError("AddDesk: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            bool deskAdded = locationRepository.AddDesk(deskInformation.DeskName, deskInformation.LocationName);

            if (!deskAdded)
            {
                logger.LogInformation("AddDesk: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            logger.LogInformation("AddDesk: Status 201, Created");
            return StatusCode(StatusCodes.Status201Created);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpDelete("RemoveDesk")]
    public IActionResult RemoveDesk([FromHeader] string token, [FromBody] DeskInformation deskInformation)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("RemoveDesk: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogInformation("RemoveDesk: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            User? user = userRepository.GetUser(username);

            if (user == null)
            {
                logger.LogInformation("RemoveDesk: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            if (user.IsAdmin == false)
            {
                logger.LogError("RemoveDesk: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            bool deskRemoved = locationRepository.RemoveDesk(deskInformation.DeskName, deskInformation.LocationName);

            if (!deskRemoved)
            {
                logger.LogInformation("RemoveDesk: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            logger.LogInformation("RemoveDesk: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
