using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AvansMeals.Domain.Entities;
using AvansMeals.Domain.Entities;
using AvansMeals.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AvansMeals.Infrastructure.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Canteen> Canteens => Set<Canteen>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Package> Packages => Set<Package>();
    public DbSet<PackageProduct> PackageProducts => Set<PackageProduct>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<PackageProduct>()
            .HasKey(pp => new { pp.PackageId, pp.ProductId });

        builder.Entity<Package>()
    .HasOne(p => p.Canteen)
    .WithMany()
    .HasForeignKey(p => p.CanteenId)
    .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<ApplicationUser>()
            .HasOne(u => u.Canteen)
            .WithMany()
            .HasForeignKey(u => u.CanteenId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
