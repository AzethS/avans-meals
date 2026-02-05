using AvansMeals.Application.Services;
using AvansMeals.Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;



namespace AvansMeals.Api.Controllers;

[ApiController]
[Route("api/packages/{packageId:int}/reservation")]
public class ReservationsController : ControllerBase
{
    private readonly PackageReservationService _service;
    private readonly UserManager<ApplicationUser> _userManager;

    public ReservationsController(
        PackageReservationService service,
        UserManager<ApplicationUser> userManager)
    {
        _service = service;
        _userManager = userManager;
    }

    [Authorize(Roles = "Student")]
    [HttpPost]
    public async Task<IActionResult> Reserve(int packageId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(studentId))
            return Unauthorized();

        var student = await _userManager.FindByIdAsync(studentId);
        if (student == null)
            return Unauthorized();

       
        var now = DateTime.Now;
        var dob = student.DateOfBirth;
        if (dob == null)
            return BadRequest(new { message = "Student date of birth is missing" });

        var studentIs18Plus = dob.Value.AddYears(18) <= now.Date;

        var result = _service.TryReservePackage(packageId, studentId, studentIs18Plus, now);

        if (result.Success)
            return Created($"/api/packages/{packageId}/reservation", new { message = "Reserved" });

        return result.Reason switch
        {
            ReservationFailReason.PackageNotFound => NotFound(new { message = result.Message ?? "Package not found" }),
            ReservationFailReason.AlreadyReserved => Conflict(new { message = result.Message ?? "Package already reserved" }),
            ReservationFailReason.AlreadyHasReservationToday => Conflict(new { message = result.Message ?? "Already has a reservation today" }),
            ReservationFailReason.OutsidePickupWindow => BadRequest(new { message = result.Message ?? "Outside pickup window" }),
            ReservationFailReason.Under18ForAlcohol => Forbid(),
            _ => BadRequest(new { message = result.Message ?? "Reservation failed" })
        };
    }

    [Authorize(Roles = "Student")]
    [HttpDelete]
    public IActionResult Cancel(int packageId)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(studentId))
            return Unauthorized();

        var now = DateTime.Now;
        var ok = _service.TryCancelReservation(packageId, studentId, now);

        if (!ok)
            return BadRequest(new { message = "Cannot cancel reservation" });

        return NoContent();
    }
}
