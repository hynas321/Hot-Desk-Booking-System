using Dotnet.Server.Managers;
using Dotnet.Server.Http;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Server.Services;
using Dotnet.Server.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using dotnet_server.Models.Constants;

namespace Dotnet.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<UserController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly IDeskService _deskService;
    private readonly ISessionTokenManager _sessionTokenManager;
    private readonly IHashManager _hashManager;

    public UserController(
        ILogger<UserController> logger,
        IConfiguration configuration,
        IUserService userService,
        IDeskService deskService,
        ISessionTokenManager sessionTokenManager,
        IHashManager hashManager)
    {
        _logger = logger;
        _configuration = configuration;
        _userService = userService;
        _deskService = deskService;
        _sessionTokenManager = sessionTokenManager;
        _hashManager = hashManager;
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] UserCredentials userCredentials, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state.");
        }

        if (await _userService.GetUserAsync(userCredentials.Username, cancellationToken) != null)
        {
            return Conflict("User already exists.");
        }

        var newUser = new User
        {
            Username = userCredentials.Username,
            Password = _hashManager.HashPassword(userCredentials.Password),
            Role = UserRole.User
        };

        await _userService.AddUserAsync(newUser, cancellationToken);

        return CreatedAtAction(nameof(GetUserInfo), new { username = newUser.Username }, newUser);
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpDelete("Remove")]
    public async Task<IActionResult> Remove([FromBody] string username, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state.");
        }

        var existingUser = await _userService.GetUserAsync(username, cancellationToken);

        if (existingUser?.Username == null)
        {
            return NotFound("User not found.");
        }

        await _userService.RemoveUserAsync(existingUser.Username, cancellationToken);
        return Ok();
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

        var token = _sessionTokenManager.CreateToken(userCredentials.Username, user.Role);
        var output = new TokenOutput { Token = token };

        return Ok(JsonHelper.Serialize(output));
    }

    [Authorize]
    [HttpPut("LogOut")]
    public IActionResult LogOut()
    {
        return Ok();
    }

    [Authorize(Roles = "Admin")]
    [HttpPut("SetAdmin")]
    public async Task<IActionResult> SetAdmin([FromBody] UserAdminStatus userAdminStatus, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest("Invalid model state.");
        }

        var user = await _userService.GetUserAsync(userAdminStatus.Username, cancellationToken);
        if (user == null)
        {
            return NotFound("User not found.");
        }

        user.Role = userAdminStatus.IsAdmin ? UserRole.Admin : UserRole.User;

        var statusSet = await _userService.UpdateUserAsync(user, cancellationToken);

        if (!statusSet)
        {
            return NotFound("Failed to update user.");
        }

        _logger.LogInformation($"SetAdmin: {userAdminStatus.Username} IsAdmin - {userAdminStatus.IsAdmin}");
        return Ok();
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpGet("IsAdmin/{username}")]
    public async Task<IActionResult> IsAdmin([FromRoute] string username, CancellationToken cancellationToken = default)
    {
        var user = await _userService.GetUserAsync(username, cancellationToken);

        if (user == null)
        {
            return NotFound("User not found.");
        }

        return Ok(user.Role == UserRole.Admin);
    }

    [Authorize]
    [HttpGet("GetBooking")]
    public async Task<IActionResult> GetBookedDeskInfo(CancellationToken cancellationToken = default)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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

    [Authorize(Roles = UserRole.Admin)]
    [HttpGet("GetAll")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
    {
        var users = await _userService.GetAllUsersAsync(cancellationToken);

        return Ok(users);
    }

    [Authorize]
    [HttpGet("GetUserInfo")]
    public async Task<IActionResult> GetUserInfo(CancellationToken cancellationToken = default)
    {
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
            Username = user.Username,
            IsAdmin = user.Role == UserRole.Admin,
            BookedDesk = deskDTO,
            BookedDeskLocation = bookedDesk?.Location?.LocationName
        };

        return Ok(JsonHelper.Serialize(output));
    }
}
