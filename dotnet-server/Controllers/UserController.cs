using Dotnet.Server.Managers;
using Dotnet.Server.Database;
using Dotnet.Server.Http;
using Dotnet.Server.Configuration;
using Dotnet.Server.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> logger;
    private readonly IConfiguration configuration;
    private readonly UserRepository userRepository;
    private readonly DeskRepository deskRepository;
    private readonly SessionTokenManager tokenManager;
    private readonly HashManager hashManager;

    #nullable disable
    public UserController(
        ILogger<UserController> logger,
        IConfiguration configuration,
        UserRepository userRepository,
        DeskRepository deskRepository,
        SessionTokenManager tokenManager,
        HashManager hashManager
    )
    {
        this.logger = logger;
        this.configuration = configuration;
        this.userRepository = userRepository;
        this.deskRepository = deskRepository;
        this.tokenManager = tokenManager;
        this.hashManager = hashManager;
    }
    #nullable enable

    [HttpPost("Add")]
    public IActionResult Add([FromHeader] string token, [FromBody] UserCredentials userCredentials)
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

                User? user = userRepository.GetUser(username);

                if (user == null || user.IsAdmin == false)
                {
                    logger.LogError("Add: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            bool userExists = userRepository.CheckIfUserExists(userCredentials.Username);

            if (userExists)
            {
                logger.LogInformation("Add: Status 409, Conflict");
                return StatusCode(StatusCodes.Status409Conflict);
            }

            UserCredentials hashedUserCredentials = new UserCredentials()
            {
                Username = userCredentials.Username,
                Password = hashManager.HashPassword(userCredentials.Password)
            };

            userRepository.AddUser(hashedUserCredentials);

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
    public IActionResult Remove([FromHeader] string token, [FromBody] UserUsername userToRemove)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Remove: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (token != configuration[Config.GlobalAdminToken]) {
                string? username = tokenManager.GetUsername(token);

                if (username == null)
                {
                    logger.LogError("Add: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                User? user = userRepository.GetUser(username);

                if (user == null || user.IsAdmin == false)
                {
                    logger.LogError("Add: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            bool userExists = userRepository.CheckIfUserExists(userToRemove.Username);

            if (!userExists)
            {
                logger.LogInformation("Remove: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            bool isUserRemoved = userRepository.RemoveUser(userToRemove.Username);

            if (!isUserRemoved)
            {
                logger.LogInformation("Remove: 500, Internal server error");
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

    [HttpGet("GetBooking")]
    public IActionResult GetBookedDeskInfo([FromHeader] string token)
    {
        try
        {
            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogError("GetBookedDeskInfo: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            User? user = userRepository.GetUser(username);

            if (user == null)
            {
                logger.LogError("GetBookedDeskInfo: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            DeskInformation? bookingInfo = userRepository.GetBookedDeskInformation(username);

            if (bookingInfo == null)
            {
                logger.LogInformation("GetBookedDeskInfo: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            logger.LogInformation("GetBookedDeskInfo: Status 200, OK", JsonHelper.Serialize(bookingInfo));
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

            bool isPasswordCorrect = hashManager.VerifyPassword(userCredentials.Password, user.Password);

            if (!isPasswordCorrect)
            {
                logger.LogError("LogIn: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            SessionTokenManager tokenManager = new SessionTokenManager();
            string token = tokenManager.CreateToken(userCredentials.Username);
            TokenOutput output = new TokenOutput
            {
                Token = token
            };

            logger.LogInformation("LogIn: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(output));
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

    [HttpGet("IsAdmin/{username}")]
    public IActionResult IsAdmin([FromHeader] string globalAdminToken, [FromRoute] string username)
    {
        try
        {
            if (globalAdminToken != configuration[Config.GlobalAdminToken])
            {
                logger.LogInformation("IsAdmin: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

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

    [HttpGet("GetUserInfo")]
    public IActionResult GetUserInfo([FromHeader] string token)
    {
        try
        {
            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogError("IsAdmin: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            User? user = userRepository.GetUser(username);

            if (user == null)
            {
                logger.LogError("IsAdmin: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            List<Desk> desks = deskRepository.GetAllDesks();
            Desk? bookedDesk = desks.FirstOrDefault(d => d.Booking?.Username == username);
            ClientsideDesk? clientsideDesk = null;
            
            if (bookedDesk != null)
            {
                #nullable disable
                clientsideDesk = new ClientsideDesk()
                {
                    DeskName = bookedDesk.DeskName,
                    IsEnabled = bookedDesk.IsEnabled,
                    Username = username,
                    StartTime = bookedDesk.Booking.StartTime.Value.ToString("dd-MM-yyyy"),
                    EndTime = bookedDesk.Booking.EndTime.Value.ToString("dd-MM-yyyy")
                };
                #nullable enable
            }

            UserInfoOutput output = new UserInfoOutput()
            {
                Username = user.Username,
                IsAdmin = user.IsAdmin,
                BookedDesk = clientsideDesk,
                BookedDeskLocation = bookedDesk?.Location?.LocationName
            };

            logger.LogInformation("IsAdmin: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(output));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("SetAdmin")]
    public IActionResult SetAdmin(
        [FromHeader] string token,
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

            if (token != configuration[Config.GlobalAdminToken]) {
                string? username = tokenManager.GetUsername(token);

                if (username == null)
                {
                    logger.LogError("SetAdmin: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                User? user = userRepository.GetUser(username);

                if (user == null || user.IsAdmin == false)
                {
                    logger.LogError("SetAdmin: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
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