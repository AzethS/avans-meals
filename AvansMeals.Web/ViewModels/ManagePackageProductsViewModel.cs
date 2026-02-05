using AvansMeals.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvansMeals.Web.ViewModels;

public class ManagePackageProductsViewModel
{
    public int PackageId { get; set; }
    public string PackageName { get; set; } = "";

    public List<Product> CurrentProducts { get; set; } = new();
    public List<SelectListItem> AllProducts { get; set; } = new();

    public int SelectedProductId { get; set; }
}
