using Dotnet.Server.Managers;
using Dotnet.Server.Repositories;
using Dotnet.Server.Http;
using Dotnet.Server.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingController : ControllerBase
{
    private readonly ILogger<BookingController> logger;
    private readonly IDeskRepository deskRepository;
    private readonly IBookingRepository bookingRepository;
    private readonly ISessionTokenManager tokenManager;

    public BookingController(
        ILogger<BookingController> logger,
        IDeskRepository deskRepository,
        IBookingRepository bookingRepository,
        ISessionTokenManager tokenManager
    )
    {
        this.logger = logger;
        this.deskRepository = deskRepository;
        this.bookingRepository = bookingRepository;
        this.tokenManager = tokenManager;
    }

    [HttpPut("Book")]
    public async Task<IActionResult> Book([FromHeader] string token, [FromBody] BookingInformation bookingInfo, CancellationToken cancellationToken = default)
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

            Desk desk = await deskRepository.GetDeskAsync(deskInfo, cancellationToken);

            if (desk == null)
            {
                logger.LogInformation("Book: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            ClientsideDesk? deskDTO = await bookingRepository.BookDeskAsync(username, bookingInfo, cancellationToken);

            if (deskDTO == null)
            {
                logger.LogInformation("Book: Status 500, Internal server error");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            return StatusCode(StatusCodes.Status200OK, JsonHelper.Serialize(deskDTO));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.ToString());
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
                logger.LogError("Unbook: Status 400, Bad Request");
                return StatusCode(StatusCodes.Status400BadRequest);
            }

            string? username = tokenManager.GetUsername(token);

            if (username == null)
            {
                logger.LogInformation("Unbook: Status 401, Unauthorized");
                return StatusCode(StatusCodes.Status401Unauthorized);
            }

            Desk desk = await deskRepository.GetDeskAsync(deskInfo, cancellationToken);

            if (desk == null)
            {
                logger.LogInformation("Unbook: Status 404, Not found");
                return StatusCode(StatusCodes.Status404NotFound);
            }

            ClientsideDesk? clientsideDesk = await bookingRepository.UnBookDeskAsync(deskInfo, cancellationToken);

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