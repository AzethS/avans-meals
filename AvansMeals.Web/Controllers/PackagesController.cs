using Microsoft.AspNetCore.Mvc;
using AvansMeals.Application.Interfaces;
using AvansMeals.Web.ViewModels;
using AvansMeals.Application.Services;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using AvansMeals.Infrastructure.Identity;

namespace AvansMeals.Web.Controllers;

public class PackagesController : Controller
{
    private readonly IPackageRepository _packageRepository;
    private readonly PackageReservationService _reservationService;
    private readonly UserManager<ApplicationUser> _userManager;
    public PackagesController(
        IPackageRepository packageRepository,
        PackageReservationService reservationService,
        UserManager<ApplicationUser> userManager)
    {
        _packageRepository = packageRepository;
        _reservationService = reservationService;
        _userManager = userManager;
    }


    // GET
    public IActionResult Index(string? city, string? mealType)
    {
        var now = DateTime.Now;

        var query = _packageRepository.GetAvailablePackages();

        if (!string.IsNullOrWhiteSpace(city))
        {
            query = query.Where(p => p.City.ToString() == city);
        }

        if (!string.IsNullOrWhiteSpace(mealType))
        {
            query = query.Where(p => p.MealType.ToString() == mealType);
        }

        ViewBag.SelectedCity = city;
        ViewBag.SelectedMealType = mealType;

        var packages = query
            .Select(p => new PackageViewModel
            {
                Id = p.Id,
                Name = p.Name,
                City = p.City.ToString(),
                MealType = p.MealType.ToString(),
                Price = p.Price,
                Is18Plus = p.Is18Plus,
                PickupFrom = p.PickupFrom,
                PickupUntil = p.PickupUntil,
                CanReserve = now < p.PickupFrom
            })
            .ToList();

        return View(packages);
    }


    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> Reserve(int id)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var user = await _userManager.FindByIdAsync(studentId);
        var is18Plus = user?.DateOfBirth != null && CalculateAge(user.DateOfBirth.Value, DateTime.Now) >= 18;

        var result = _reservationService.TryReservePackage(id, studentId, is18Plus, DateTime.Now);

        if (result.Success)
        {
            TempData["Success"] = "Reservering gelukt!";
        }
        else
        {
            TempData["Error"] = result.Reason switch
            {
                ReservationFailReason.PackageNotFound => "Package bestaat niet (meer).",
                ReservationFailReason.AlreadyReserved => "Deze package is al gereserveerd.",
                ReservationFailReason.OutsidePickupWindow => "Reserveren niet mogelijk: het ophaalmoment is al gestart.",
                ReservationFailReason.Under18ForAlcohol => "Je bent nog geen 18, deze package bevat alcohol.",
                ReservationFailReason.AlreadyHasReservationToday => "Je hebt vandaag al een reservering.",
                _ => "Reserveren niet mogelijk."
            };
        }

        return RedirectToAction(nameof(Index));
    }

    private static int CalculateAge(DateTime dob, DateTime now)
    {
        var age = now.Year - dob.Year;
        if (dob.Date > now.Date.AddYears(-age)) age--;
        return age;
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MyReservations()
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var packages = await _packageRepository.GetReservedByStudentAsync(studentId);

        var vm = packages.Select(p => new PackageViewModel
        {
            Id = p.Id,
            Name = p.Name,
            City = p.City.ToString(),
            MealType = p.MealType.ToString(),
            Price = p.Price,
            Is18Plus = p.Is18Plus,
            IsReserved = true,
            PickupFrom = p.PickupFrom,
            PickupUntil = p.PickupUntil,
        });

        return View(vm);
    }


    [HttpPost]
    [Authorize(Roles = "Student")]
    public IActionResult Cancel(int id)
    {
        var studentId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(studentId)) return Unauthorized();

        var success = _reservationService.TryCancelReservation(id, studentId, DateTime.Now);

        if (!success)
            TempData["Error"] = "Annuleren niet mogelijk (misschien is het ophaalmoment al begonnen).";
        else
            TempData["Success"] = "Reservering geannuleerd.";

        return RedirectToAction(nameof(MyReservations));
    }


    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var package = await _packageRepository.GetByIdWithProductsAsync(id);
        if (package == null) return NotFound();

        return View(package);
    }


}
