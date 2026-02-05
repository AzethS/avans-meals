using AvansMeals.Application.Interfaces;
using AvansMeals.Application.Services;
using AvansMeals.Infrastructure.Data;
using AvansMeals.Infrastructure.Identity;
using AvansMeals.Infrastructure.Repositories;
using AvansMeals.Web.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using AvansMeals.Web.Services;
using AvansMeals.Domain.Enums;


var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();


// DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false; 
    })
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddSingleton<IEmailSender, DummyEmailSender>();


// DI voor app
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<PackageReservationService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    // Seed data
    DataSeeder.Seed(context);

    // Seed roles
    await IdentitySeeder.SeedRolesAsync(roleManager);

    // Seed employee + koppel aan bestaande kantine

    var bredaLaId = context.Canteens.First(c => c.City == City.Breda && c.LocationCode == "LA").Id;
    var bredaLdId = context.Canteens.First(c => c.City == City.Breda && c.LocationCode == "LD").Id;
    var tilburgTbId = context.Canteens.First(c => c.City == City.Tilburg && c.LocationCode == "TB").Id;

    await IdentitySeeder.SeedCanteenEmployeeAsync(userManager, roleManager, "canteen.la@avans.nl", "Avans123!", bredaLaId);
    await IdentitySeeder.SeedCanteenEmployeeAsync(userManager, roleManager, "canteen.ld@avans.nl", "Avans123!", bredaLdId);
    await IdentitySeeder.SeedCanteenEmployeeAsync(userManager, roleManager, "canteen.tb@avans.nl", "Avans123!", tilburgTbId);

}


// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();


app.Run();
