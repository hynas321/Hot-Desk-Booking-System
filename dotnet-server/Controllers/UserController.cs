using Dotnet.Server.Authentication;
using Dotnet.Server.Database;
using Dotnet.Server.Http;
using dotnet_server.Configuration;
using Microsoft.AspNetCore.Mvc;

namespace dotnet_server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> logger;
    private readonly IConfiguration configuration;
    private readonly UserRepository userRepository;
    private readonly SessionTokenManager tokenManager;

    public UserController(
        ILogger<UserController> logger,
        IConfiguration configuration,
        UserRepository userRepository,
        SessionTokenManager tokenManager
    )
    {
        this.logger = logger;
        this.configuration = configuration;
        this.userRepository = userRepository;
        this.tokenManager = tokenManager;
    }

    [HttpPost("Add")]
    public IActionResult Add([FromBody] UserCredentials userCredentials)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Add: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            bool userExists = userRepository.CheckIfUserExists(userCredentials.Username);

            if (userExists)
            {
                logger.LogInformation("Add: Status 200, OK");
                return StatusCode(StatusCodes.Status200OK);
            }

            userRepository.AddUser(userCredentials);

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
    public IActionResult Remove([FromHeader] string globalAdminToken, [FromBody] UserUsername username)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Remove: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (globalAdminToken != configuration[Config.GlobalAdminToken])
            {
                logger.LogError("Remove: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            bool userRemoved = userRepository.RemoveUser(username.Username);

            if (!userRemoved)
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

            List<User> users = await userRepository.GetAllUsersAsync();

            logger.LogInformation("GetAll: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, users);

        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetAllSessions")]
    public IActionResult GetAllSessions([FromHeader] string globalAdminToken)
    {
        try
        {
            if (globalAdminToken != configuration[Config.GlobalAdminToken])
            {
                logger.LogError("GetAllSessions: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            Dictionary<string, string> sessions = tokenManager.GetAllSessions();

            logger.LogInformation("GetAllSessions: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, sessions);

        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPost("LogIn")]
    public IActionResult LogIn([FromBody] UserCredentials userCredentials)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("LogIn: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            User? user = userRepository.GetUser(userCredentials.Username);

            if (user == null)
            {
                logger.LogInformation("LogIn: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            if (userCredentials.Password != user.Password)
            {
                logger.LogError("LogIn: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            SessionTokenManager tokenManager = new SessionTokenManager();
            string token = tokenManager.CreateToken(userCredentials.Username);

            logger.LogInformation("LogIn: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, token);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("LogOut")]
    public IActionResult LogOut([FromHeader] string token)
    {
        try
        {
            if (token == null || token == "")
            {
                logger.LogError("LogOut: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            bool userLoggedOut = tokenManager.RemoveToken(token);

            logger.LogInformation("LogOut: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, userLoggedOut);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("IsAdmin")]
    public IActionResult IsAdmin(string username)
    {
        try
        {
            User? user = userRepository.GetUser(username);

            if (user == null)
            {
                logger.LogInformation("IsAdmin: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            logger.LogInformation("IsAdmin: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, user.IsAdmin);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("SetAdmin")]
    public IActionResult SetAdmin(
        [FromHeader] string globalAdminToken,
        [FromBody] UserAdminStatus userAdminStatus
    )
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("SetAdmin: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (globalAdminToken != configuration[Config.GlobalAdminToken])
            {
                logger.LogError("SetAdmin: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            bool statusSet = userRepository.SetAdminStatus(userAdminStatus.Username, userAdminStatus.IsAdmin);

            if (!statusSet)
            {
                logger.LogInformation("SetAdmin: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            logger.LogInformation("SetAdmin: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}