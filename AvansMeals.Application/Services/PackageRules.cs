using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvansMeals.Application.Services
{
    public static class PackageRules
    {
        public static bool IsPickupDateWithinTwoDays(DateTime pickupFrom, DateTime today)
            => pickupFrom.Date <= today.Date.AddDays(2);
    }

}
