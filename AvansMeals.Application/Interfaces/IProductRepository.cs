using AvansMeals.Domain.Entities;

namespace AvansMeals.Application.Interfaces;

public interface IProductRepository
{
    Task<List<Product>> GetAllAsync();
    Product? GetById(int id);
    void Add(Product product);
    void Save();
}
