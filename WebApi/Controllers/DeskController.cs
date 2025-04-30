using WebApi.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Helpers;
using Microsoft.AspNetCore.Authorization;
using WebApi.Models.Constants;
using WebApi.Services.Abstractions;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DeskController : ControllerBase
{
    private readonly ILogger<DeskController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserService _userService;
    private readonly IDeskService _deskService;

    public DeskController(
        ILogger<DeskController> logger,
        IConfiguration configuration,
        IUserService userService,
        IDeskService deskService
    )
    {
        _logger = logger;
        _configuration = configuration;
        _userService = userService;
        _deskService = deskService;
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpPost("Add")]
    public async Task<IActionResult> Add([FromBody] DeskInformation deskInfo, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var existingDesk = await _deskService.GetDeskAsync(deskInfo, cancellationToken);

        if (existingDesk != null)
        {
            return Conflict();
        }

        bool deskAdded = await _deskService.AddDeskAsync(deskInfo, cancellationToken);

        if (!deskAdded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return CreatedAtAction(nameof(Add), new { deskInfo.DeskName, deskInfo.LocationName });
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpDelete("Remove")]
    public async Task<IActionResult> Remove([FromBody] DeskInformation deskInfo, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var desk = await _deskService.GetDeskAsync(deskInfo, cancellationToken);

        if (desk == null)
        {
            return NotFound();
        }

        bool isRemoved = await _deskService.RemoveDeskAsync(deskInfo, cancellationToken);

        if (!isRemoved)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok();
    }

    [Authorize(Roles = UserRole.Admin)]
    [HttpPut("SetDeskAvailability")]
    public async Task<IActionResult> SetDeskAvailability([FromBody] DeskAvailabilityInformation deskAvailabilityInfo, CancellationToken cancellationToken = default)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest();
        }

        var deskInfo = new DeskInformation
        {
            DeskName = deskAvailabilityInfo.DeskName,
            LocationName = deskAvailabilityInfo.LocationName
        };

        var desk = await _deskService.GetDeskAsync(deskInfo, cancellationToken);

        if (desk == null)
        {
            return NotFound();
        }

        var updatedDeskDTO = await _deskService.SetDeskAvailabilityAsync(deskInfo, deskAvailabilityInfo.IsEnabled, cancellationToken);

        if (updatedDeskDTO == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        return Ok(JsonHelper.Serialize(updatedDeskDTO));
    }
}
