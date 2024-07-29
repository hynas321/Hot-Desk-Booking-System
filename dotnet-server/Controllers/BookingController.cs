using Dotnet.Server.Managers;
using Dotnet.Server.Http;
using Dotnet.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Server.Services;

namespace Dotnet.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly ILogger<BookingController> _logger;
    private readonly IDeskService _deskService;
    private readonly IUserService _userService;
    private readonly IBookingService _bookingService;
    private readonly ISessionTokenManager _tokenManager;

    public BookingController(
        ILogger<BookingController> logger,
        IDeskService deskRepository,
        IUserService userService,
        IBookingService bookingService,
        ISessionTokenManager tokenManager
    )
    {
        _logger = logger;
        _deskService = deskRepository;
        _userService = userService;
        _bookingService = bookingService;
        _tokenManager = tokenManager;
    }

    [HttpPut("Book")]
    public async Task<IActionResult> Book([FromHeader] string token, [FromBody] BookingInformation bookingInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Book: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            string? username = _tokenManager.GetUsername(token);

            if (username == null)
            {
                _logger.LogInformation("Book: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            DeskInformation deskInfo = new DeskInformation()
            {
                DeskName = bookingInfo.DeskName,
                LocationName = bookingInfo.LocationName
            };

            Desk? desk = await _deskService.GetDeskAsync(deskInfo, cancellationToken);

            if (desk == null)
            {
                _logger.LogInformation("Book: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            User? user = await _userService.GetUserAsync(username, cancellationToken);

            if (user == null)
            {
                _logger.LogInformation("Book: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            DeskDTO? deskDTO = await _bookingService.AddBookingAsync(user, bookingInfo, cancellationToken);

            if (deskDTO == null)
            {
                _logger.LogInformation("Book: Status 500, Internal server error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(deskDTO));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [HttpPut("Unbook")]
    public async Task<IActionResult> Unbook([FromHeader] string token, [FromBody] DeskInformation deskInfo, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Unbook: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            string? username = _tokenManager.GetUsername(token);

            if (username == null)
            {
                _logger.LogInformation("Unbook: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            Desk? desk = await _deskService.GetDeskAsync(deskInfo, cancellationToken);

            if (desk == null)
            {
                _logger.LogInformation("Unbook: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            DeskDTO? clientsideDesk = await _bookingService.RemoveBookingAsync(deskInfo, cancellationToken);

            if (clientsideDesk == null)
            {
                _logger.LogInformation("Unbook: Status 500, Internal server error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(clientsideDesk));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}