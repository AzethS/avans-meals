using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvansMeals.Application.Interfaces;
using AvansMeals.Domain.Entities;
using AvansMeals.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace AvansMeals.Infrastructure.Repositories;

public class PackageRepository : IPackageRepository
{
    private readonly ApplicationDbContext _context;

    public PackageRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Package> GetAvailablePackages()
    {
        return _context.Packages
            .Include(p => p.PackageProducts)
                .ThenInclude(pp => pp.Product)
            .Where(p => p.ReservedByStudentId == null)
            .ToList();
    }

    public Package? GetById(int id)
    {
        return _context.Packages
            .Include(p => p.PackageProducts)
            .FirstOrDefault(p => p.Id == id);
    }
    public async Task<List<Package>> GetReservedByStudentAsync(string studentId)
    {
        return await _context.Packages
            .Include(p => p.PackageProducts)
                .ThenInclude(pp => pp.Product)
            .Where(p => p.ReservedByStudentId == studentId)
            .ToListAsync();
    }
    public async Task<List<Package>> GetAllAsync()
    {
        return await _context.Packages
            .Include(p => p.PackageProducts)
                .ThenInclude(pp => pp.Product)
            .ToListAsync();
    }
    public void Add(Package package)
    {
        _context.Packages.Add(package);
    }

    public void Remove(Package package)
    {
        _context.Packages.Remove(package);
    }
    public async Task<Package?> GetByIdWithProductsAsync(int id)
    {
        return await _context.Packages
            .Include(p => p.PackageProducts)
                .ThenInclude(pp => pp.Product)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    public bool StudentHasReservationOnDate(string studentId, DateTime date)
    {
        var start = date.Date;
        var end = start.AddDays(1);

        return _context.Packages.Any(p =>
            p.ReservedByStudentId == studentId &&
            p.PickupFrom >= start && p.PickupFrom < end);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
