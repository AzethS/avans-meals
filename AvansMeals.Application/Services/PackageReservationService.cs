using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvansMeals.Application.Interfaces;


namespace AvansMeals.Application.Services;

public class PackageReservationService
{
    private readonly IPackageRepository _repository;



    public PackageReservationService(IPackageRepository repository)
    {
        _repository = repository;
    }

    public ReservationResult TryReservePackage(int packageId, string studentId, bool studentIs18Plus, DateTime now)
    {
        var package = _repository.GetById(packageId);
        if (package == null)
            return ReservationResult.Fail(ReservationFailReason.PackageNotFound);

        // max 1 reservering per dag
        if (_repository.StudentHasReservationOnDate(studentId, now))
            return ReservationResult.Fail(ReservationFailReason.AlreadyHasReservationToday);

        // package al gereserveerd door iemand anders
        if (package.ReservedByStudentId != null)
            return ReservationResult.Fail(ReservationFailReason.AlreadyReserved);

        // reserveren alleen VOOR pickup start
        if (now >= package.PickupFrom)
            return ReservationResult.Fail(ReservationFailReason.OutsidePickupWindow); 

        // 18+ rule
        if (package.Is18Plus && !studentIs18Plus)
            return ReservationResult.Fail(ReservationFailReason.Under18ForAlcohol);

        package.Reserve(studentId);
        _repository.Save();

        return ReservationResult.Ok();
    }


    public bool TryCancelReservation(int packageId, string studentId, DateTime now)
    {
        var package = _repository.GetById(packageId);
        if (package == null) return false;

        if (!package.CanBeCancelledBy(studentId, now))
            return false;

        package.CancelReservation();
        _repository.Save();
        return true;
    }


}
