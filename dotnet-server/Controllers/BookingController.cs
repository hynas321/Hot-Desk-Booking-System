using Dotnet.Server.Managers;
using Dotnet.Server.Http;
using Dotnet.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Server.Services;

namespace Dotnet.Server.Controllers
{
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
            IDeskService deskService,
            IUserService userService,
            IBookingService bookingService,
            ISessionTokenManager tokenManager
        )
        {
            _logger = logger;
            _deskService = deskService;
            _userService = userService;
            _bookingService = bookingService;
            _tokenManager = tokenManager;
        }

        [HttpPut("Book")]
        public async Task<IActionResult> Book([FromHeader] string token, [FromBody] BookingInformation bookingInfo, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Book: Status 400, Bad Request");
                return BadRequest();
            }

            var username = _tokenManager.GetUsername(token);
            if (username == null)
            {
                _logger.LogInformation("Book: Status 401, Unauthorized");
                return Unauthorized();
            }

            var deskInformation = new DeskInformation { DeskName = bookingInfo.DeskName, LocationName = bookingInfo.LocationName };
            var desk = await _deskService.GetDeskAsync(deskInformation, cancellationToken);
            if (desk == null)
            {
                return NotFound();
            }

            var user = await _userService.GetUserAsync(username, cancellationToken);
            if (user == null)
            {
                _logger.LogInformation("Book: Status 401, Unauthorized");
                return Unauthorized();
            }

            var deskDTO = await _bookingService.AddBookingAsync(user, bookingInfo, cancellationToken);
            if (deskDTO == null)
            {
                _logger.LogInformation("Book: Status 500, Internal Server Error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(JsonHelper.Serialize(deskDTO));
        }

        [HttpPut("Unbook")]
        public async Task<IActionResult> Unbook([FromHeader] string token, [FromBody] DeskInformation deskInfo, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Unbook: Status 400, Bad Request");
                return BadRequest();
            }

            var username = _tokenManager.GetUsername(token);
            if (username == null)
            {
                _logger.LogInformation("Unbook: Status 401, Unauthorized");
                return Unauthorized();
            }

            var desk = await _deskService.GetDeskAsync(deskInfo, cancellationToken);
            if (desk == null)
            {
                return NotFound();
            }

            var deskDTO = await _bookingService.RemoveBookingAsync(deskInfo, cancellationToken);
            if (deskDTO == null)
            {
                _logger.LogInformation("Unbook: Status 500, Internal Server Error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return Ok(JsonHelper.Serialize(deskDTO));
        }
    }
}
