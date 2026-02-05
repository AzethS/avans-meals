using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvansMeals.Domain.Entities;

namespace AvansMeals.Application.Interfaces;

public interface IPackageRepository
{
    IEnumerable<Package> GetAvailablePackages();

    Package? GetById(int id);
    Task<List<Package>> GetReservedByStudentAsync(string studentId);
    Task<List<Package>> GetAllAsync();
    void Add(AvansMeals.Domain.Entities.Package package);
    void Remove(AvansMeals.Domain.Entities.Package package);
    Task<AvansMeals.Domain.Entities.Package?> GetByIdWithProductsAsync(int id);

    bool StudentHasReservationOnDate(string studentId, DateTime date);

    void Save();
}

