using AvansMeals.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvansMeals.Domain.Entities
{
    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public City City { get; set; }
        public DateTime PickupFrom { get; set; }
        public DateTime PickupUntil { get; set; }
        public decimal Price { get; set; }
        public MealType MealType { get; set; }
        public bool Is18Plus { get; private set; }

        public string? ReservedByStudentId { get; set; }
        public int? CanteenId { get; set; }
        public Canteen? Canteen { get; set; }


        public ICollection<PackageProduct> PackageProducts { get; set; } = new List<PackageProduct>();
        public bool CanBeReservedBy(string studentId, DateTime now, bool studentIs18Plus)
        {
            if (ReservedByStudentId != null)
                return false;

            if (now >= PickupFrom)
                return false;

            if (Is18Plus && !studentIs18Plus)
                return false;

            return true;
        }



        public void Reserve(string studentId)
        {
            ReservedByStudentId = studentId;
        }

        public void Recalculate18Plus()
        {
            Is18Plus = PackageProducts.Any(pp =>
                pp.Product != null && pp.Product.ContainsAlcohol);
        }
        public bool CanBeCancelledBy(string studentId, DateTime now)
        {
            if (ReservedByStudentId == null)
                return false;

            if (ReservedByStudentId != studentId)
                return false;

            if (now >= PickupFrom)
                return false;

            return true;
        }

        public void CancelReservation()
        {
            ReservedByStudentId = null;
        }

    }
}
