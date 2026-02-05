using AvansMeals.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvansMeals.Domain.Entities
{
    public class Canteen
    {
        public int Id { get; set; }
        public City City { get; set; }
        public string LocationCode { get; set; } = null!;
        public bool OffersWarmMeals { get; set; }
    }
}
