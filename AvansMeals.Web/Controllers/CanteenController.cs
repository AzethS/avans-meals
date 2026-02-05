using AvansMeals.Application.Interfaces;
using AvansMeals.Domain.Entities;
using AvansMeals.Infrastructure.Data;
using AvansMeals.Infrastructure.Identity;
using AvansMeals.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvansMeals.Web.Controllers;

[Authorize(Roles = "CanteenEmployee")]
public class CanteenController : Controller
{
    private readonly IPackageRepository _repo;
    private readonly IProductRepository _productRepo;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public CanteenController(
        IPackageRepository repo,
        IProductRepository productRepo,
        UserManager<ApplicationUser> userManager,
        ApplicationDbContext context)
    {
        _repo = repo;
        _productRepo = productRepo;
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Packages()
    {
        var packages = await _repo.GetAllAsync();
        return View(packages);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View(new PackageCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(PackageCreateViewModel vm)
    {
        var user = await _userManager.GetUserAsync(User);
        var canteen = _context.Canteens.First(c => c.Id == user.CanteenId.Value);

        if (user?.CanteenId == null)
        {
            ModelState.AddModelError("", "Je account heeft geen kantine-locatie. Neem contact op met een beheerder.");
            return View(vm);
        }

        var maxDate = DateTime.Today.AddDays(2);
        if (vm.PickupFrom.Date > maxDate)
        {
            ModelState.AddModelError(nameof(vm.PickupFrom), "Je mag een pakket maximaal 2 dagen vooruit aanbieden.");
        }

        if (vm.PickupUntil <= vm.PickupFrom)
        {
            ModelState.AddModelError(nameof(vm.PickupUntil), "PickupUntil moet na PickupFrom liggen.");
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var package = new Package
        {
            Name = vm.Name,
            MealType = vm.MealType,
            Price = vm.Price,
            PickupFrom = vm.PickupFrom,
            PickupUntil = vm.PickupUntil,
            CanteenId = user.CanteenId.Value,
            City = canteen.City
        };

        _repo.Add(package);
        _repo.Save();

        return RedirectToAction(nameof(Packages));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var package = _repo.GetById(id);
        if (package == null) return NotFound();

        var vm = new PackageEditViewModel
        {
            Id = package.Id,
            Name = package.Name,
            MealType = package.MealType,
            Price = package.Price,
            PickupFrom = package.PickupFrom,
            PickupUntil = package.PickupUntil
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(PackageEditViewModel vm)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user?.CanteenId == null)
        {
            return Forbid();
        }

        var maxDate = DateTime.Today.AddDays(2);
        if (vm.PickupFrom.Date > maxDate)
        {
            ModelState.AddModelError(nameof(vm.PickupFrom), "Je mag een pakket maximaal 2 dagen vooruit aanbieden.");
        }

        if (vm.PickupUntil <= vm.PickupFrom)
        {
            ModelState.AddModelError(nameof(vm.PickupUntil), "PickupUntil moet na PickupFrom liggen.");
        }

        if (!ModelState.IsValid)
        {
            return View(vm);
        }

        var package = _repo.GetById(vm.Id);
        if (package == null) return NotFound();

        // Alleen eigen kantine mag editen
        if (package.CanteenId != user.CanteenId.Value)
        {
            return Forbid();
        }

        // Voorkom edit als al gereserveerd
        if (package.ReservedByStudentId != null)
        {
            TempData["Error"] = "Je kunt een gereserveerd package niet aanpassen.";
            return RedirectToAction(nameof(Packages));
        }

        package.Name = vm.Name;
        package.MealType = vm.MealType;
        package.Price = vm.Price;
        package.PickupFrom = vm.PickupFrom;
        package.PickupUntil = vm.PickupUntil;
        var canteen = _context.Canteens.First(c => c.Id == user.CanteenId.Value);
        package.City = canteen.City;
        package.CanteenId = user.CanteenId.Value;

        _repo.Save();
        return RedirectToAction(nameof(Packages));
    }

    [HttpGet]
    public IActionResult Delete(int id)
    {
        var package = _repo.GetById(id);
        if (package == null) return NotFound();

        // niet verwijderen als gereserveerd
        if (package.ReservedByStudentId != null)
        {
            TempData["Error"] = "Je kunt een gereserveerd package niet verwijderen.";
            return RedirectToAction(nameof(Packages));
        }

        return View(package);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult DeleteConfirmed(int id)
    {
        var package = _repo.GetById(id);
        if (package == null) return NotFound();

        if (package.ReservedByStudentId != null)
        {
            TempData["Error"] = "Je kunt een gereserveerd package niet verwijderen.";
            return RedirectToAction(nameof(Packages));
        }

        _repo.Remove(package);
        _repo.Save();

        TempData["Success"] = "Package verwijderd.";
        return RedirectToAction(nameof(Packages));
    }

    [HttpGet]
    public async Task<IActionResult> ManageProducts(int packageId)
    {
        var package = await _repo.GetByIdWithProductsAsync(packageId);
        if (package == null) return NotFound();

        var allProducts = await _productRepo.GetAllAsync();

        var vm = new ManagePackageProductsViewModel
        {
            PackageId = package.Id,
            PackageName = package.Name,
            CurrentProducts = package.PackageProducts.Select(pp => pp.Product).ToList(),
            AllProducts = allProducts
                .Select(p => new SelectListItem { Value = p.Id.ToString(), Text = $"{p.Name} {(p.ContainsAlcohol ? "(Alcohol)" : "")}" })
                .ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProduct(int packageId, int productId)
    {
        var package = await _repo.GetByIdWithProductsAsync(packageId);
        if (package == null) return NotFound();

        // Haal het product op uit de database
        var product = _productRepo.GetById(productId);
        if (product == null) return NotFound();

        // voorkom dubbele koppeling
        if (!package.PackageProducts.Any(pp => pp.ProductId == productId))
        {
            package.PackageProducts.Add(new PackageProduct
            {
                PackageId = packageId,
                ProductId = productId,
                Product = product
            });

            // Update Is18Plus correct
            package.Recalculate18Plus();

            // Save changes
            _repo.Save();
        }

        return RedirectToAction(nameof(ManageProducts), new { packageId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveProduct(int packageId, int productId)
    {
        var package = await _repo.GetByIdWithProductsAsync(packageId);
        if (package == null) return NotFound();

        var link = package.PackageProducts.FirstOrDefault(pp => pp.ProductId == productId);
        if (link != null)
        {
            package.PackageProducts.Remove(link);
            package.Recalculate18Plus();
            _repo.Save();
        }

        return RedirectToAction(nameof(ManageProducts), new { packageId });
    }

    [HttpGet]
    public IActionResult CreateProduct(int packageId)
    {
        ViewBag.PackageId = packageId;
        return View(new ProductCreateViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult CreateProduct(int packageId, ProductCreateViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.PackageId = packageId;
            return View(vm);
        }

        var product = new AvansMeals.Domain.Entities.Product
        {
            Name = vm.Name,
            ContainsAlcohol = vm.ContainsAlcohol
        };

        _productRepo.Add(product);
        _productRepo.Save();

        // terug naar manage products van hetzelfde package
        return RedirectToAction(nameof(ManageProducts), new { packageId });
    }



}
