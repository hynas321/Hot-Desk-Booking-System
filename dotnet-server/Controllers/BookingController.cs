using Dotnet.Server.Managers;
using Dotnet.Server.Database;
using Dotnet.Server.Http;
using Dotnet.Server.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly ILogger<BookingController> logger;
    private readonly DeskRepository deskRepository;
    private readonly BookingRepository bookingRepository;
    private readonly SessionTokenManager tokenManager;

    public BookingController(
        ILogger<BookingController> logger,
        DeskRepository deskRepository,
        BookingRepository bookingRepository,
        SessionTokenManager tokenManager
    )
    {
        this.logger = logger;
        this.deskRepository = deskRepository;
        this.bookingRepository = bookingRepository;
        this.tokenManager = tokenManager;
    }

    [HttpPut("Book")]
    public IActionResult Book([FromHeader] string token, [FromBody] BookingInformation bookingInfo)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Book: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogInformation("Book: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            DeskInformation deskInfo = new DeskInformation()
            {
                DeskName = bookingInfo.DeskName,
                LocationName = bookingInfo.LocationName
            };

            bool deskExists = deskRepository.CheckIfDeskExists(deskInfo);

            if (!deskExists)
            {
                logger.LogInformation("Book: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            ClientsideDesk? clientsideDesk = bookingRepository.BookDesk(username, bookingInfo);

            if (clientsideDesk == null)
            {
                logger.LogInformation("Book: Status 500, Internal server error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(clientsideDesk));
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
        try
        {
            if (!ModelState.IsValid)
            {
                logger.LogError("Unbook: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogInformation("Unbook: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            bool deskExists = deskRepository.CheckIfDeskExists(deskInfo);

            if (!deskExists)
            {
                logger.LogInformation("Unbook: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            ClientsideDesk? clientsideDesk = bookingRepository.UnbookDesk(deskInfo);

            if (clientsideDesk == null)
            {
                logger.LogInformation("Unbook: Status 500, Internal server error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(clientsideDesk));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}