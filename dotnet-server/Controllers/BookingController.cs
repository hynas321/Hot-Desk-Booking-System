using Dotnet.Server.Http;
using Dotnet.Server.Helpers;
using Microsoft.AspNetCore.Mvc;
using Dotnet.Server.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        public BookingController(
            ILogger<BookingController> logger,
            IDeskService deskService,
            IUserService userService,
            IBookingService bookingService
        )
        {
            _logger = logger;
            _deskService = deskService;
            _userService = userService;
            _bookingService = bookingService;
        }

        [Authorize]
        [HttpPut("Book")]
        public async Task<IActionResult> Book([FromBody] BookingInformation bookingInfo, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Book: Status 400, Bad Request");
                return BadRequest();
            }

            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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

        [Authorize]
        [HttpPut("Unbook")]
        public async Task<IActionResult> Unbook([FromBody] DeskInformation deskInfo, CancellationToken cancellationToken = default)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Unbook: Status 400, Bad Request");
                return BadRequest();
            }

            var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
