using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvansMeals.Application.Services
{
    public sealed record ReservationResult(bool Success, ReservationFailReason Reason, string? Message = null)
    {
        public static ReservationResult Ok() => new(true, ReservationFailReason.None);
        public static ReservationResult Fail(ReservationFailReason reason, string? message = null)
            => new(false, reason, message);
    }
}
