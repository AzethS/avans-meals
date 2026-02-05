using AvansMeals.Application.Interfaces;
using AvansMeals.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace AvansMeals.Api.Controllers;

[ApiController]
[Route("api/packages")]
public class PackagesController : ControllerBase
{
    private readonly IPackageRepository _repo;

    public PackagesController(IPackageRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult Get([FromQuery] City? city, [FromQuery] MealType? mealType)
    {
        var packages = _repo.GetAvailablePackages();

        if (city.HasValue)
            packages = packages.Where(p => p.City == city.Value);

        if (mealType.HasValue)
            packages = packages.Where(p => p.MealType == mealType.Value);

        var result = packages.Select(p => new
        {
            p.Id,
            p.Name,
            City = p.City.ToString(),
            MealType = p.MealType.ToString(),
            p.Price,
            p.Is18Plus,
            p.PickupFrom,
            p.PickupUntil,
            Reserved = p.ReservedByStudentId != null
        });

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var p = await _repo.GetByIdWithProductsAsync(id);
        if (p == null) return NotFound();

        return Ok(new
        {
            p.Id,
            p.Name,
            City = p.City.ToString(),
            MealType = p.MealType.ToString(),
            p.Price,
            p.Is18Plus,
            p.PickupFrom,
            p.PickupUntil,
            Products = p.PackageProducts.Select(pp => new
            {
                pp.ProductId,
                Name = pp.Product!.Name,
                pp.Product.ContainsAlcohol
            })
        });
    }
}
