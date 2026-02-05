using AvansMeals.Application.Services;
using AvansMeals.Application.Interfaces;
using AvansMeals.Domain.Entities;
using FluentAssertions;
using Moq;

namespace AvansMeals.Tests.Services;

public class PackageReservationServiceTests
{
    private static Package CreatePackage(int id, DateTime pickupFrom, DateTime pickupUntil, bool make18Plus = false)
    {
        var p = new Package
        {
            Id = id,
            PickupFrom = pickupFrom,
            PickupUntil = pickupUntil,
            ReservedByStudentId = null
        };

        if (make18Plus)
        {
            var alcoholProduct = new Product { Id = 999, Name = "Beer", ContainsAlcohol = true };
            p.PackageProducts.Add(new PackageProduct
            {
                ProductId = alcoholProduct.Id,
                Product = alcoholProduct,
                PackageId = p.Id
            });

            p.Recalculate18Plus();
        }

        return p;
    }

    [Fact]
    public void TryReservePackage_PackageIsAvailable_ShouldReservePackage()
    {
        // Arrange
        var now = DateTime.Today.AddHours(9);

        var package = new Package
        {
            Id = 1,
            PickupFrom = DateTime.Today.AddHours(10),
            PickupUntil = DateTime.Today.AddHours(12),
            ReservedByStudentId = null,
        };

        var repoMock = new Mock<IPackageRepository>();
        repoMock.Setup(r => r.GetById(1)).Returns(package);
        repoMock.Setup(r => r.StudentHasReservationOnDate("student-1", now)).Returns(false);

        var service = new PackageReservationService(repoMock.Object);

        // Act
        var result = service.TryReservePackage(1, "student-1", studentIs18Plus: true, now: now);

        // Assert
        result.Success.Should().BeTrue();
        result.Reason.Should().Be(ReservationFailReason.None);
        package.ReservedByStudentId.Should().Be("student-1");
        repoMock.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public void TryReservePackage_PackageNotFound_ShouldFail()
    {
        // Arrange
        var now = DateTime.Today.AddHours(9);

        var repoMock = new Mock<IPackageRepository>();
        repoMock.Setup(r => r.GetById(999)).Returns((Package?)null);

        var service = new PackageReservationService(repoMock.Object);

        // Act
        var result = service.TryReservePackage(999, "student-1", true, now);

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ReservationFailReason.PackageNotFound);
        repoMock.Verify(r => r.Save(), Times.Never);
    }

    [Fact]
    public void TryReservePackage_AlreadyReserved_ShouldFail()
    {
        // Arrange
        var now = DateTime.Today.AddHours(9);

        var package = new Package
        {
            Id = 1,
            PickupFrom = DateTime.Today.AddHours(10),
            PickupUntil = DateTime.Today.AddHours(12),
            ReservedByStudentId = "other-student",
        };

        var repoMock = new Mock<IPackageRepository>();
        repoMock.Setup(r => r.GetById(1)).Returns(package);
        repoMock.Setup(r => r.StudentHasReservationOnDate("student-1", now)).Returns(false);

        var service = new PackageReservationService(repoMock.Object);

        // Act
        var result = service.TryReservePackage(1, "student-1", true, now);

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ReservationFailReason.AlreadyReserved);
        repoMock.Verify(r => r.Save(), Times.Never);
    }

    [Fact]
    public void TryReservePackage_AlreadyHasReservationToday_ShouldFail()
    {
        // Arrange
        var now = DateTime.Today.AddHours(9);

        var package = new Package
        {
            Id = 1,
            PickupFrom = DateTime.Today.AddHours(10),
            PickupUntil = DateTime.Today.AddHours(12),
            ReservedByStudentId = null,
        };

        var repoMock = new Mock<IPackageRepository>();
        repoMock.Setup(r => r.GetById(1)).Returns(package);
        repoMock.Setup(r => r.StudentHasReservationOnDate("student-1", now)).Returns(true);

        var service = new PackageReservationService(repoMock.Object);

        // Act
        var result = service.TryReservePackage(1, "student-1", true, now);

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ReservationFailReason.AlreadyHasReservationToday);
        repoMock.Verify(r => r.Save(), Times.Never);
    }

    [Fact]
    public void TryReservePackage_WhenNowIsAtOrAfterPickupFrom_ShouldFailOutsidePickupWindow()
    {
        // Arrange
        var now = DateTime.Today.AddHours(10); // pickup start

        var package = new Package
        {
            Id = 1,
            PickupFrom = DateTime.Today.AddHours(10),
            PickupUntil = DateTime.Today.AddHours(12),
            ReservedByStudentId = null,
        };

        var repoMock = new Mock<IPackageRepository>();
        repoMock.Setup(r => r.GetById(1)).Returns(package);
        repoMock.Setup(r => r.StudentHasReservationOnDate("student-1", now)).Returns(false);

        var service = new PackageReservationService(repoMock.Object);

        // Act
        var result = service.TryReservePackage(1, "student-1", true, now);

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ReservationFailReason.OutsidePickupWindow);
        repoMock.Verify(r => r.Save(), Times.Never);
    }

    [Fact]
    public void TryReservePackage_Under18ForAlcoholPackage_ShouldFail()
    {
        // Arrange
        var now = DateTime.Today.AddHours(9);

        var package = CreatePackage(
            id: 1,
            pickupFrom: DateTime.Today.AddHours(10),
            pickupUntil: DateTime.Today.AddHours(12),
            make18Plus: true);

        var repoMock = new Mock<IPackageRepository>();
        repoMock.Setup(r => r.GetById(1)).Returns(package);
        repoMock.Setup(r => r.StudentHasReservationOnDate("student-1", now)).Returns(false);

        var service = new PackageReservationService(repoMock.Object);

        // Act
        var result = service.TryReservePackage(1, "student-1", studentIs18Plus: false, now: now);

        // Assert
        result.Success.Should().BeFalse();
        result.Reason.Should().Be(ReservationFailReason.Under18ForAlcohol);
        repoMock.Verify(r => r.Save(), Times.Never);
    }

    [Fact]
    public void TryCancelReservation_WhenRuleAllows_ShouldCancelAndSave()
    {
        // Arrange
        var now = DateTime.Today.AddHours(9);

        var package = new Package
        {
            Id = 1,
            PickupFrom = DateTime.Today.AddHours(10),
            PickupUntil = DateTime.Today.AddHours(12),
            ReservedByStudentId = "student-1"
        };

        var repoMock = new Mock<IPackageRepository>();
        repoMock.Setup(r => r.GetById(1)).Returns(package);

        var service = new PackageReservationService(repoMock.Object);

        // Act
        var ok = service.TryCancelReservation(1, "student-1", now);

        // Assert
        ok.Should().BeTrue();
        package.ReservedByStudentId.Should().BeNull();
        repoMock.Verify(r => r.Save(), Times.Once);
    }

    [Fact]
    public void TryCancelReservation_WhenPackageNotFound_ShouldReturnFalse()
    {
        // Arrange
        var now = DateTime.Today.AddHours(9);

        var repoMock = new Mock<IPackageRepository>();
        repoMock.Setup(r => r.GetById(1)).Returns((Package?)null);

        var service = new PackageReservationService(repoMock.Object);

        // Act
        var ok = service.TryCancelReservation(1, "student-1", now);

        // Assert
        ok.Should().BeFalse();
        repoMock.Verify(r => r.Save(), Times.Never);
    }
}
