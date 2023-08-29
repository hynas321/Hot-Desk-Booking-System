using Dotnet.Server.Managers;
using Dotnet.Server.Database;
using Dotnet.Server.Http;
using dotnet_server.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeskController : ControllerBase
{
    private readonly ILogger<LocationController> logger;
    private readonly IConfiguration configuration;
    private readonly UserRepository userRepository;
    private readonly DeskRepository deskRepository;
    private readonly SessionTokenManager tokenManager;

    public DeskController(
        ILogger<LocationController> logger,
        IConfiguration configuration,
        UserRepository userRepository,
        DeskRepository deskRepository,
        SessionTokenManager tokenManager
    )
    {
        this.logger = logger;
        this.configuration = configuration;
        this.userRepository = userRepository;
        this.deskRepository = deskRepository;
        this.tokenManager = tokenManager;
    }

    [HttpPost("Add")]
    public IActionResult Add([FromHeader] string token, [FromBody] DeskInformation deskInformation)
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

            if (user == null || user.IsAdmin)
            {
                logger.LogError("Add: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            bool deskAdded = deskRepository.AddDesk(deskInformation);

            if (!deskAdded)
            {
                logger.LogInformation("Add: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
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
    public IActionResult Remove([FromHeader] string token, [FromBody] DeskInformation deskInformation)
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

            if (user == null || user.IsAdmin == false)
            {
                logger.LogError("Remove: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            bool deskRemoved = deskRepository.RemoveDesk(deskInformation);

            if (!deskRemoved)
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
    public async Task<IActionResult> GetAll([FromHeader] string globalAdminToken)
    {
        try
        {
            if (globalAdminToken != configuration[Config.GlobalAdminToken])
            {
                logger.LogError("GetAll: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            List<Desk> desks = await deskRepository.GetAllDesksAsync();

            logger.LogInformation("GetAll: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, desks);

        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("Book")]
    public IActionResult Book([FromHeader] string token, [FromBody] BookingInformation bookingInformation)
    {
        //To finish
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("BookDesk: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogInformation("BookDesk: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            deskRepository.BookDesk(username, bookingInformation);

            return StatusCode(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("Unbook")]
    public IActionResult Unbook([FromHeader] string token, [FromBody] DeskInformation deskInfo)
    {
        //To finish
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("BookDesk: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogInformation("BookDesk: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            deskRepository.UnbookDesk(deskInfo);

            return StatusCode(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}