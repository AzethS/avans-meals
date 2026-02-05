using System.ComponentModel.DataAnnotations;
using AvansMeals.Domain.Enums;

namespace AvansMeals.Web.ViewModels;

public class PackageCreateViewModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;


    [Required]
    public MealType MealType { get; set; }

    [Range(0.01, 999.99)]
    public decimal Price { get; set; }

    [Required]
    [Display(Name = "Ophalen van")]
    public DateTime PickupFrom { get; set; } = DateTime.Now.AddHours(1);

    [Required]
    [Display(Name = "Ophalen tot")]
    public DateTime PickupUntil { get; set; } = DateTime.Now.AddHours(3);
}
