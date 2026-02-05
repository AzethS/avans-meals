using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using AvansMeals.Domain.Entities;




namespace AvansMeals.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? DateOfBirth { get; set; }
        public int? CanteenId { get; set; }
        public Canteen? Canteen { get; set; }

    }
}
