using System;
using System.Collections.Generic;
using System.Linq;

using AvansMeals.Domain.Entities;
using AvansMeals.Domain.Enums;

namespace AvansMeals.Infrastructure.Data;

public static class DataSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        // Seed niet opnieuw als er al data staat
        if (context.Canteens.Any() || context.Products.Any() || context.Packages.Any())
            return;

        var canteens = new List<Canteen>
        {
            new Canteen { City = City.Breda,   LocationCode = "LA", OffersWarmMeals = true  },
            new Canteen { City = City.Breda,   LocationCode = "LD", OffersWarmMeals = false },
            new Canteen { City = City.Tilburg, LocationCode = "TB", OffersWarmMeals = true  },
        };

        // Products 
        var products = new List<Product>
        {
            new Product { Name = "Broodje kaas", ContainsAlcohol = false },
            new Product { Name = "Appel",        ContainsAlcohol = false },
            new Product { Name = "Yoghurt",      ContainsAlcohol = false },
            new Product { Name = "Salade",       ContainsAlcohol = false },
            new Product { Name = "Bier",         ContainsAlcohol = true  },
            new Product { Name = "Wijn",         ContainsAlcohol = true  },
        };

        context.Canteens.AddRange(canteens);
        context.Products.AddRange(products);
        context.SaveChanges(); 


        Package MakePackage(
            Canteen canteen,
            string name,
            MealType mealType,
            decimal price,
            DateTime from,
            DateTime until,
            params Product[] packageProducts)
        {
            if (until <= from)
                until = from.AddHours(2);

            var p = new Package
            {
                Name = name,
                CanteenId = canteen.Id,
                City = canteen.City,       
                MealType = mealType,
                Price = price,
                PickupFrom = from,
                PickupUntil = until
            };

            foreach (var prod in packageProducts)
            {
                p.PackageProducts.Add(new PackageProduct { Product = prod });
            }

            p.Recalculate18Plus();
            return p;
        }

        var now = DateTime.Now;

     
        var bredaLa = canteens.First(c => c.City == City.Breda && c.LocationCode == "LA");
        var bredaLd = canteens.First(c => c.City == City.Breda && c.LocationCode == "LD");
        var tilburgTb = canteens.First(c => c.City == City.Tilburg && c.LocationCode == "TB");

        // Packages
        var packages = new List<Package>
        {
            MakePackage(bredaLa, "Lunchpakket Basic", MealType.Bread, 3.50m,
                now.AddMinutes(-11), now.AddHours(13), products[0], products[1]),

            MakePackage(bredaLa, "Lunchpakket Luxe", MealType.Bread, 4.75m,
                now.AddMinutes(-11), now.AddHours(14), products[0], products[2]),

            MakePackage(bredaLa, "Salade Box", MealType.Salad, 4.25m,
                now.AddHours(10), now.AddHours(12), products[3], products[1]),

            MakePackage(bredaLa, "Warm meal deal", MealType.WarmMeal, 5.50m,
                now.AddHours(16), now.AddHours(18), products[2], products[1]),

            MakePackage(bredaLa, "Borrel pakket (18+)", MealType.Bread, 6.50m,
                now.AddHours(17), now.AddHours(19), products[0], products[4]), 

            MakePackage(bredaLa, "Wijn & snack (18+)", MealType.Salad, 7.25m,
                now.AddHours(18), now.AddHours(20), products[3], products[5]),
            MakePackage(bredaLd, "LD - Lunchpakket", MealType.Bread, 3.25m,
    now.AddHours(11), now.AddHours(13), products[0], products[1]),

MakePackage(bredaLd, "LD - Salade", MealType.Salad, 4.10m,
    now.AddHours(12), now.AddHours(14), products[3], products[2]),

MakePackage(tilburgTb, "TB - Warm Meal", MealType.WarmMeal, 6.00m,
    now.AddHours(16), now.AddHours(18), products[2], products[1]),
        };

        context.Packages.AddRange(packages);
        context.SaveChanges();
    }
}
