using Dotnet.Server.Managers;
using Dotnet.Server.Repositories;
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
    private readonly IUserRepository userRepository;
    private readonly IDeskRepository deskRepository;
    private readonly ISessionTokenManager tokenManager;
    private readonly IHashManager hashManager;

    #nullable disable
    public UserController(
        ILogger<UserController> logger,
        IConfiguration configuration,
        IUserRepository userRepository,
        IDeskRepository deskRepository,
        ISessionTokenManager tokenManager,
        IHashManager hashManager
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
    public async Task<IActionResult> Add([FromHeader] string token, [FromBody] UserCredentials userCredentials, CancellationToken cancellationToken = default)
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

            User? existingUser = await userRepository.GetUserAsync(userCredentials.Username, cancellationToken);

            if (existingUser == null)
            {
                logger.LogInformation("Add: Status 409, Conflict");
                return StatusCode(StatusCodes.Status409Conflict);
            }

            User newUser = new User()
            {
                Username = userCredentials.Username,
                Password = hashManager.HashPassword(userCredentials.Password),
                IsAdmin = false
            };

            await userRepository.AddUserAsync(newUser, cancellationToken);

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
    public async Task<IActionResult> Remove([FromHeader] string token, [FromBody] UserUsername userToRemove, CancellationToken cancellationToken = default)
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

                User? user = await userRepository.GetUserAsync(username, cancellationToken);

                if (user == null || user.IsAdmin == false)
                {
                    logger.LogError("Add: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            User? existingUser = await userRepository.GetUserAsync(userToRemove.Username, cancellationToken);

            if (existingUser == null)
            {
                logger.LogInformation("Remove: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            bool isUserRemoved = await userRepository.RemoveUserAsync(userToRemove.Username, cancellationToken);

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
    public async Task<IActionResult> GetBookedDeskInfo([FromHeader] string token, CancellationToken cancellationToken = default)
    {
        try
        {
            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogError("GetBookedDeskInfo: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            User? user = await userRepository.GetUserAsync(username, cancellationToken);

            if (user == null)
            {
                logger.LogError("GetBookedDeskInfo: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            DeskInformation? bookingInfo = await userRepository.GetBookedDeskInformationAsync(username, cancellationToken);

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
    public async Task<IActionResult> GetAll([FromHeader] string globalAdminToken, CancellationToken cancellationToken = default)
    {
        try
        {
            if (globalAdminToken != configuration[Config.GlobalAdminToken])
            {
                logger.LogError("GetAll: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            List<User> users = await userRepository.GetAllUsersAsync(cancellationToken);

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
    public IActionResult GetAllSessions([FromHeader] string globalAdminToken, CancellationToken cancellationToken = default)
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
    public async Task<IActionResult> LogIn([FromBody] UserCredentials userCredentials, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("LogIn: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            User? user = await userRepository.GetUserAsync(userCredentials.Username, cancellationToken);

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
    public IActionResult LogOut([FromHeader] string token, CancellationToken cancellationToken = default)
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
    public async Task<IActionResult> IsAdmin([FromHeader] string globalAdminToken, [FromRoute] string username, CancellationToken cancellationToken = default)
    {
        try
        {
            if (globalAdminToken != configuration[Config.GlobalAdminToken])
            {
                logger.LogInformation("IsAdmin: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            User? user = await userRepository.GetUserAsync(username, cancellationToken);

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
    public async Task<IActionResult> GetUserInfo([FromHeader] string token, CancellationToken cancellationToken = default)
    {
        try
        {
            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogError("IsAdmin: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            User? user = await userRepository.GetUserAsync(username, cancellationToken);

            if (user == null)
            {
                logger.LogError("IsAdmin: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            List<Desk> desks = await deskRepository.GetAllDesksAsync(cancellationToken);
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
    public async Task<IActionResult> SetAdmin([FromHeader] string token, [FromBody] UserAdminStatus userAdminStatus, CancellationToken cancellationToken = default)
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

                User? user = await userRepository.GetUserAsync(username, cancellationToken);

                if (user == null || user.IsAdmin == false)
                {
                    logger.LogError("SetAdmin: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            bool statusSet = await userRepository.SetAdminStatus(userAdminStatus.Username, userAdminStatus.IsAdmin, cancellationToken);

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