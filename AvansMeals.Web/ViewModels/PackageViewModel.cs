namespace AvansMeals.Web.ViewModels;

public class PackageViewModel
{
    public string Name { get; set; } = null!;
    public string City { get; set; } = null!;
    public string MealType { get; set; } = null!;
    public decimal Price { get; set; }
    public bool Is18Plus { get; set; }
    public int Id { get; set; }
    public bool IsReserved { get; set; }
    public bool CanReserve { get; set; }
    public DateTime PickupFrom { get; set; }
    public DateTime PickupUntil { get; set; }



}
