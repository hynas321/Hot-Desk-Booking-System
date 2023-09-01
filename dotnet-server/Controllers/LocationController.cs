using Dotnet.Server.Managers;
using Dotnet.Server.Database;
using Dotnet.Server.Http;
using Dotnet.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Server.Configuration;

namespace Dotnet.Server.Controllers;

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

            if (token != configuration[Config.GlobalAdminToken])
            {
                string? username = tokenManager.GetUsername(token);

                if (username == null)
                {
                    logger.LogInformation("Add: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                User? user = userRepository.GetUser(username);

                if (user == null || user.IsAdmin == false)
                {
                    logger.LogError("Add: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            bool locationExists = locationRepository.CheckIfLocationExists(locationName.Name);

            if (locationExists)
            {
                logger.LogInformation("Add: Status 409, Conflict");
                return StatusCode(StatusCodes.Status409Conflict);
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

            if (token != configuration[Config.GlobalAdminToken])
            {
                string? username = tokenManager.GetUsername(token);

                if (username == null)
                {
                    logger.LogInformation("Remove: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                User? user = userRepository.GetUser(username);

                if (user == null || user.IsAdmin == false)
                {
                    logger.LogError("Remove: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            bool locationExists = locationRepository.CheckIfLocationExists(locationName.Name);

            if (!locationExists)
            {
                logger.LogInformation("Remove: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            bool isLocationRemoved = locationRepository.RemoveLocation(locationName.Name);

            if (!isLocationRemoved)
            {
                logger.LogInformation("Remove: Status 500, Internal server error");
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

    [HttpGet("GetDesks/{locationName}")]
    public IActionResult GetDesks([FromHeader] string token, [FromRoute] string locationName)
    {
        try
        {
            Location? location = locationRepository.GetLocation(locationName);
            List<ClientsideDesk> clientsideDesks = new List<ClientsideDesk>();
            string? username = null;
            User? user = null;

            if (token != configuration[Config.GlobalAdminToken])
            {
                username = tokenManager.GetUsername(token);

                if (username == null)
                {
                    logger.LogInformation("GetDesks: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                user = userRepository.GetUser(username);

                if (user == null)
                {
                    logger.LogError("GetDesks: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            if (location == null)
            {
                logger.LogInformation("GetDesks: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            foreach (var desk in location.Desks)
            {
                #nullable disable

                string usernameProperty = "-";

                if (user.IsAdmin == true || desk?.Booking?.Username == username) {
                    usernameProperty = desk.Booking?.Username;
                }
                else {
                    usernameProperty = desk.Booking?.Username == null ? null : "-";
                }

                clientsideDesks.Add(
                    new ClientsideDesk()
                    {
                        DeskName = desk.DeskName,
                        IsEnabled = desk.IsEnabled,
                        Username = usernameProperty,
                        StartTime =
                            desk.Booking.StartTime.HasValue ?
                            desk.Booking.StartTime.Value.ToString("dd-MM-yyyy")
                            : null,
                        EndTime =
                            desk.Booking.EndTime.HasValue ?
                            desk.Booking.EndTime.Value.ToString("dd-MM-yyyy")
                            : null
                    }
                );
            }

            logger.LogInformation("GetDesks: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(clientsideDesks));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetAllNames")]
    public async Task<IActionResult> GetAllNames()
    {
        try
        {
            List<Location> locations = await locationRepository.GetAllLocationsAsync();
            List<ClientsideLocation> clientsideLocations = new List<ClientsideLocation>();
            
            foreach(var location in locations)
            {
                clientsideLocations.Add(
                    new ClientsideLocation()
                    {
                        LocationName = location.LocationName,
                        TotalDeskCount = location.Desks.Count,
                        AvailableDeskCount =
                            location.Desks.Where(x => x.Booking?.Username == null && x.IsEnabled).Count()
                    }
                );
            }

            logger.LogInformation("GetAllNames: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(clientsideLocations));

        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromHeader] string globalAdminToken)
    {
        try
        {
            if (globalAdminToken != configuration[Config.GlobalAdminToken])
            {
                logger.LogError("GetAll: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            List<Location> locations = await locationRepository.GetAllLocationsAsync();

            logger.LogInformation("GetAll: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(locations));

        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
