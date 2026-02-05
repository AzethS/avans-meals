using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvansMeals.Domain.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public bool ContainsAlcohol { get; set; }
        public string? PhotoUrl { get; set; }
    }
}
