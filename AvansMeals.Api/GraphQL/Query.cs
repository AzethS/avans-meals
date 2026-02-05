using AvansMeals.Domain.Entities;
using AvansMeals.Infrastructure.Data;
using HotChocolate;
using HotChocolate.Data;

namespace AvansMeals.Api.GraphQL;

public class Query
{
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public IQueryable<Package> GetPackages([Service] ApplicationDbContext db)
        => db.Packages;
}
