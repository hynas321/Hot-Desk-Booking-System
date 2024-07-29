using Dotnet.Server.Managers;
using Dotnet.Server.Repositories;
using Dotnet.Server.Http;
using Dotnet.Server.Configuration;
using Dotnet.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Server.Services;

namespace Dotnet.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly IDeskService _deskService;
    private readonly ISessionTokenManager _tokenManager;
    private readonly IHashManager _hashManager;

    #nullable disable
    public UserController(
        ILogger<UserController> logger,
        IConfiguration configuration,
        IUserService userService,
        IDeskService deskService,
        ISessionTokenManager tokenManager,
        IHashManager hashManager
    )
    {
        _logger = logger;
        _configuration = configuration;
        _userService = userService;
        _deskService = deskService;
        _tokenManager = tokenManager;
        _hashManager = hashManager;
    }
    #nullable enable

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromHeader] string token, [FromBody] UserCredentials userCredentials, CancellationToken cancellationToken = default)
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

            User? existingUser = await _userService.GetUserAsync(userCredentials.Username, cancellationToken);

            if (existingUser == null)
            {
                _logger.LogInformation("Add: Status 409, Conflict");
                return StatusCode(StatusCodes.Status409Conflict);
            }

            User newUser = new User()
            {
                UserName = userCredentials.Username,
                Password = _hashManager.HashPassword(userCredentials.Password),
                IsAdmin = false
            };

            await _userService.AddUserAsync(newUser, cancellationToken);

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
    public async Task<IActionResult> Remove([FromHeader] string token, [FromBody] UserUsername userToRemove, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Remove: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (token != _configuration[Config.GlobalAdminToken]) {
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

            User? existingUser = await _userService.GetUserAsync(userToRemove.Username, cancellationToken);

            if (existingUser == null)
            {
                _logger.LogInformation("Remove: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            await _userService.RemoveUserAsync(existingUser.UserName, cancellationToken);

            _logger.LogInformation("Remove: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetBooking")]
    public async Task<IActionResult> GetBookedDeskInfo([FromHeader] string token, CancellationToken cancellationToken = default)
    {
        try
        {
            string? username = _tokenManager.GetUsername(token);

            if (username == null)
            {
                _logger.LogError("GetBookedDeskInfo: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            User? user = await _userService.GetUserAsync(username, cancellationToken);

            if (user == null)
            {
                _logger.LogError("GetBookedDeskInfo: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            Desk? bookedDesk = user.Bookings
                .Select(b => b.Desk)
                .FirstOrDefault();

            if (bookedDesk == null)
            {
                _logger.LogInformation("GetBookedDeskInfo: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            DeskInformation bookingInfo = new DeskInformation
            {
                DeskName = bookedDesk.DeskName,
                LocationName = bookedDesk.Location.LocationName
            };

            _logger.LogInformation("GetBookedDeskInfo: Status 200, OK", JsonHelper.Serialize(bookingInfo));
            return StatusCode(StatusCodes.Status200OK, bookingInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromHeader] string globalAdminToken, CancellationToken cancellationToken = default)
    {
        try
        {
            if (globalAdminToken != _configuration[Config.GlobalAdminToken])
            {
                _logger.LogError("GetAll: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            List<User>? users = await _userService.GetAllUsersAsync(cancellationToken);

            _logger.LogInformation("GetAll: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, users);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetAllSessions")]
    public IActionResult GetAllSessions([FromHeader] string globalAdminToken, CancellationToken cancellationToken = default)
    {
        try
        {
            if (globalAdminToken != _configuration[Config.GlobalAdminToken])
            {
                _logger.LogError("GetAllSessions: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            Dictionary<string, string> sessions = _tokenManager.GetAllSessions();

            _logger.LogInformation("GetAllSessions: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, sessions);

        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
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
                _logger.LogError("LogIn: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            User? user = await _userService.GetUserAsync(userCredentials.Username, cancellationToken);

            if (user == null)
            {
                _logger.LogInformation("LogIn: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            bool isPasswordCorrect = _hashManager.VerifyPassword(userCredentials.Password, user.Password);

            if (!isPasswordCorrect)
            {
                _logger.LogError("LogIn: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            SessionTokenManager tokenManager = new SessionTokenManager();
            string token = tokenManager.CreateToken(userCredentials.Username);
            TokenOutput output = new TokenOutput
            {
                Token = token
            };

            _logger.LogInformation("LogIn: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(output));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
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
                _logger.LogError("LogOut: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            bool userLoggedOut = _tokenManager.RemoveToken(token);

            _logger.LogInformation("LogOut: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, userLoggedOut);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("IsAdmin/{username}")]
    public async Task<IActionResult> IsAdmin([FromHeader] string globalAdminToken, [FromRoute] string username, CancellationToken cancellationToken = default)
    {
        try
        {
            if (globalAdminToken != _configuration[Config.GlobalAdminToken])
            {
                _logger.LogInformation("IsAdmin: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            User? user = await _userService.GetUserAsync(username, cancellationToken);

            if (user == null)
            {
                _logger.LogInformation("IsAdmin: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            _logger.LogInformation("IsAdmin: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, user.IsAdmin);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpGet("GetUserInfo")]
    public async Task<IActionResult> GetUserInfo([FromHeader] string token, CancellationToken cancellationToken = default)
    {
        try
        {
            string? username = _tokenManager.GetUsername(token);

            if (username == null)
            {
                _logger.LogError("GetUserInfo: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            User? user = await _userService.GetUserAsync(username, cancellationToken);

            if (user == null)
            {
                _logger.LogError("GetUserInfo: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            Desk? bookedDesk = user.Bookings
                .Select(b => b.Desk)
                .FirstOrDefault();

            DeskDTO? deskDTO = null;

            if (bookedDesk != null)
            {
                deskDTO = new DeskDTO
                {
                    DeskName = bookedDesk.DeskName,
                    IsEnabled = bookedDesk.IsEnabled,
                    Username = username,
                    StartTime = bookedDesk.Bookings.First().StartTime?.ToString("dd-MM-yyyy"),
                    EndTime = bookedDesk.Bookings.First().EndTime?.ToString("dd-MM-yyyy")
                };
            }

            UserInfoOutput output = new UserInfoOutput
            {
                Username = user.UserName,
                IsAdmin = user.IsAdmin,
                BookedDesk = deskDTO,
                BookedDeskLocation = bookedDesk?.Location?.LocationName
            };

            _logger.LogInformation("GetUserInfo: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK, output);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
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
                _logger.LogError("SetAdmin: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            if (token != _configuration[Config.GlobalAdminToken]) {
                string? username = _tokenManager.GetUsername(token);

                if (username == null)
                {
                    _logger.LogError("SetAdmin: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }

                User? user = await _userService.GetUserAsync(username, cancellationToken);

                if (user == null || user.IsAdmin == false)
                {
                    _logger.LogError("SetAdmin: Status 401, Unauthorized");
                    return StatusCode(StatusCodes.Status401Unauthorized);
                }
            }

            User? userr = await _userService.GetUserAsync(userAdminStatus.Username, cancellationToken);

            if (userr == null)
            {
                _logger.LogError("SetAdmin: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            userr.IsAdmin = userAdminStatus.IsAdmin;

            bool statusSet = await _userService.UpdateUserAsync(userr, cancellationToken);

            if (!statusSet)
            {
                _logger.LogInformation("SetAdmin: Status 404, Not Found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            _logger.LogInformation("SetAdmin: Status 200, OK");
            return StatusCode(StatusCodes.Status200OK);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}