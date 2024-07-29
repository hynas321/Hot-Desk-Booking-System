using Dotnet.Server.Managers;
using Dotnet.Server.Repositories;
using Dotnet.Server.Http;
using Dotnet.Server.Configuration;
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

    public UserController(
        ILogger<UserController> logger,
        IConfiguration configuration,
        IUserService userService,
        IDeskService deskService,
        ISessionTokenManager tokenManager,
        IHashManager hashManager)
    {
        _logger = logger;
        _configuration = configuration;
        _userService = userService;
        _deskService = deskService;
        _tokenManager = tokenManager;
        _hashManager = hashManager;
    }

    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromHeader] string token, [FromBody] UserCredentials userCredentials, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state.");
        }

        if (!await IsGlobalAdminOrAuthorized(token, cancellationToken))
        {
            return Unauthorized("Unauthorized access.");
        }

        if (await _userService.GetUserAsync(userCredentials.Username, cancellationToken) != null)
        {
            return Conflict("User already exists.");
        }

        var newUser = new User
        {
            UserName = userCredentials.Username,
            Password = _hashManager.HashPassword(userCredentials.Password),
            IsAdmin = false
        };

        await _userService.AddUserAsync(newUser, cancellationToken);
        return CreatedAtAction(nameof(GetUserInfo), new { username = newUser.UserName }, newUser);
    }

    [HttpDelete("Remove")]
    public async Task<IActionResult> Remove([FromHeader] string token, [FromBody] UserUsername userToRemove, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state.");
        }

        if (!await IsGlobalAdminOrAuthorized(token, cancellationToken))
        {
            return Unauthorized("Unauthorized access.");
        }

        var existingUser = await _userService.GetUserAsync(userToRemove.Username, cancellationToken);
        if (existingUser == null)
        {
            return NotFound("User not found.");
        }

        await _userService.RemoveUserAsync(existingUser.UserName, cancellationToken);
        return Ok();
    }

    [HttpGet("GetBooking")]
    public async Task<IActionResult> GetBookedDeskInfo([FromHeader] string token, CancellationToken cancellationToken = default)
    {
        var username = _tokenManager.GetUsername(token);
        if (username == null)
        {
            return Unauthorized("Unauthorized access.");
        }

        var user = await _userService.GetUserAsync(username, cancellationToken);
        if (user == null)
        {
            return Unauthorized("Unauthorized access.");
        }

        var bookedDesk = user.Bookings.Select(b => b.Desk).FirstOrDefault();
        if (bookedDesk == null)
        {
            return NotFound("No booked desk found.");
        }

        var bookingInfo = new DeskInformation
        {
            DeskName = bookedDesk.DeskName,
            LocationName = bookedDesk.Location.LocationName
        };

        return Ok(bookingInfo);
    }

    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll([FromHeader] string globalAdminToken, CancellationToken cancellationToken = default)
    {
        if (globalAdminToken != _configuration[Config.GlobalAdminToken])
        {
            return Unauthorized("Unauthorized access.");
        }

        var users = await _userService.GetAllUsersAsync(cancellationToken);
        return Ok(users);
    }

    [HttpGet("GetAllSessions")]
    public IActionResult GetAllSessions([FromHeader] string globalAdminToken, CancellationToken cancellationToken = default)
    {
        if (globalAdminToken != _configuration[Config.GlobalAdminToken])
        {
            return Unauthorized("Unauthorized access.");
        }

        var sessions = _tokenManager.GetAllSessions();
        return Ok(sessions);
    }

    [HttpPost("LogIn")]
    public async Task<IActionResult> LogIn([FromBody] UserCredentials userCredentials, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state.");
        }

        var user = await _userService.GetUserAsync(userCredentials.Username, cancellationToken);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        if (!_hashManager.VerifyPassword(userCredentials.Password, user.Password))
        {
            return Unauthorized("Invalid credentials.");
        }

        var token = _tokenManager.CreateToken(userCredentials.Username);
        var output = new TokenOutput { Token = token };
        return Ok(output);
    }

    [HttpPut("LogOut")]
    public IActionResult LogOut([FromHeader] string token, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrEmpty(token))
        {
            return BadRequest("Token is required.");
        }

        var userLoggedOut = _tokenManager.RemoveToken(token);
        return Ok(userLoggedOut);
    }

    [HttpGet("IsAdmin/{username}")]
    public async Task<IActionResult> IsAdmin([FromHeader] string globalAdminToken, [FromRoute] string username, CancellationToken cancellationToken = default)
    {
        if (globalAdminToken != _configuration[Config.GlobalAdminToken])
        {
            return Unauthorized("Unauthorized access.");
        }

        var user = await _userService.GetUserAsync(username, cancellationToken);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(user.IsAdmin);
    }

    [HttpGet("GetUserInfo")]
    public async Task<IActionResult> GetUserInfo([FromHeader] string token, CancellationToken cancellationToken = default)
    {
        var username = _tokenManager.GetUsername(token);
        if (username == null)
        {
            return NotFound("User not found.");
        }

        var user = await _userService.GetUserAsync(username, cancellationToken);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        var bookedDesk = user.Bookings.Select(b => b.Desk).FirstOrDefault();
        var deskDTO = bookedDesk != null ? new DeskDTO
        {
            DeskName = bookedDesk.DeskName,
            IsEnabled = bookedDesk.IsEnabled,
            Username = username,
            StartTime = bookedDesk.Bookings.First().StartTime?.ToString("dd-MM-yyyy"),
            EndTime = bookedDesk.Bookings.First().EndTime?.ToString("dd-MM-yyyy")
        } : null;

        var output = new UserInfoOutput
        {
            Username = user.UserName,
            IsAdmin = user.IsAdmin,
            BookedDesk = deskDTO,
            BookedDeskLocation = bookedDesk?.Location?.LocationName
        };

        return Ok(output);
    }

    [HttpPut("SetAdmin")]
    public async Task<IActionResult> SetAdmin([FromHeader] string token, [FromBody] UserAdminStatus userAdminStatus, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state.");
        }

        if (!await IsGlobalAdminOrAuthorized(token, cancellationToken))
        {
            return Unauthorized("Unauthorized access.");
        }

        var user = await _userService.GetUserAsync(userAdminStatus.Username, cancellationToken);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        user.IsAdmin = userAdminStatus.IsAdmin;
        var statusSet = await _userService.UpdateUserAsync(user, cancellationToken);

        if (!statusSet)
        {
            return NotFound("Failed to update user.");
        }

        return Ok();
    }

    private async Task<bool> IsGlobalAdminOrAuthorized(string token, CancellationToken cancellationToken)
    {
        if (token == _configuration[Config.GlobalAdminToken])
        {
            return true;
        }

        var username = _tokenManager.GetUsername(token);
        if (username == null)
        {
            return false;
        }

        var user = await _userService.GetUserAsync(username, cancellationToken);
        return user != null && user.IsAdmin;
    }
}
