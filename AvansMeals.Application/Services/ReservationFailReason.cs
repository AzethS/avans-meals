using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvansMeals.Application.Services
{
    public enum ReservationFailReason
    {
        None = 0,
        PackageNotFound,
        AlreadyReserved,
        OutsidePickupWindow,
        Under18ForAlcohol,
        AlreadyHasReservationToday,
        MaxOnePerDay
    }
}
