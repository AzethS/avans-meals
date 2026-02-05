using System.ComponentModel.DataAnnotations;
using AvansMeals.Domain.Enums;

namespace AvansMeals.Web.ViewModels;

public class PackageEditViewModel
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;


    [Required]
    public MealType MealType { get; set; }

    [Range(0.01, 999.99)]
    public decimal Price { get; set; }

    [Required]
    public DateTime PickupFrom { get; set; }

    [Required]
    public DateTime PickupUntil { get; set; }
}
