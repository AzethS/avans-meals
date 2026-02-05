using AvansMeals.Application.Interfaces;
using AvansMeals.Domain.Entities;
using AvansMeals.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AvansMeals.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        return await _context.Products.ToListAsync();
    }

    public Product? GetById(int id)
    {
        return _context.Products.FirstOrDefault(p => p.Id == id);
    }

    public void Add(Product product)
    {
        _context.Products.Add(product);
    }

    public void Save()
    {
        _context.SaveChanges();
    }
}
