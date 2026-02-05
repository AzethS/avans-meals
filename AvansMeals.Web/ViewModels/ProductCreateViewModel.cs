using System.ComponentModel.DataAnnotations;

namespace AvansMeals.Web.ViewModels;

public class ProductCreateViewModel
{
    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Bevat alcohol")]
    public bool ContainsAlcohol { get; set; }
}
