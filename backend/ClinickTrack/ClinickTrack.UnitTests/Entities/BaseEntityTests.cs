using Xunit;
using FluentAssertions;
using ClinickCore.Entities;
using System;

namespace ClinickTrack.Tests.Entities
{
    public class BaseEntityTests
    {
        [Fact]
        public void BaseEntity_ShouldInitialize_WithDefaultValues()
        {
            // Arrange & Act
            var entity = new BaseEntity();

            // Assert
            entity.Should().NotBeNull();
            entity.Id.Should().Be(0); // Default int value
            entity.RecordDate.Should().Be(default(DateTime));
        }

        [Fact]
        public void BaseEntity_Id_CanBeSet()
        {
            // Arrange
            var entity = new BaseEntity();

            // Act
            entity.Id = 42;

            // Assert
            entity.Id.Should().Be(42);
        }

        [Fact]
        public void BaseEntity_Id_CanBeNegative()
        {
            // Arrange & Act
            var entity = new BaseEntity { Id = -1 };

            // Assert
            entity.Id.Should().Be(-1);
        }

        [Fact]
        public void BaseEntity_RecordDate_CanBeSet()
        {
            // Arrange
            var entity = new BaseEntity();
            var testDate = new DateTime(2025, 11, 19, 10, 30, 0);

            // Act
            entity.RecordDate = testDate;

            // Assert
            entity.RecordDate.Should().Be(testDate);
            entity.RecordDate.Year.Should().Be(2025);
            entity.RecordDate.Month.Should().Be(11);
            entity.RecordDate.Day.Should().Be(19);
        }

        [Fact]
        public void BaseEntity_RecordDate_CanBeCurrentTime()
        {
            // Arrange
            var beforeTime = DateTime.Now;
            var entity = new BaseEntity();

            // Act
            entity.RecordDate = DateTime.Now;
            var afterTime = DateTime.Now;

            // Assert
            entity.RecordDate.Should().BeOnOrAfter(beforeTime);
            entity.RecordDate.Should().BeOnOrBefore(afterTime);
        }

        [Fact]
        public void BaseEntity_RecordDate_CanBePastDate()
        {
            // Arrange & Act
            var pastDate = new DateTime(2020, 1, 1);
            var entity = new BaseEntity { RecordDate = pastDate };

            // Assert
            entity.RecordDate.Should().BeBefore(DateTime.Now);
            entity.RecordDate.Year.Should().Be(2020);
        }

        [Fact]
        public void BaseEntity_RecordDate_CanBeFutureDate()
        {
            // Arrange & Act
            var futureDate = DateTime.Now.AddYears(1);
            var entity = new BaseEntity { RecordDate = futureDate };

            // Assert
            entity.RecordDate.Should().BeAfter(DateTime.Now);
        }

        [Fact]
        public void BaseEntity_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange & Act
            var entity1 = new BaseEntity { Id = 1, RecordDate = new DateTime(2025, 1, 1) };
            var entity2 = new BaseEntity { Id = 2, RecordDate = new DateTime(2025, 2, 1) };

            // Assert
            entity1.Id.Should().NotBe(entity2.Id);
            entity1.RecordDate.Should().NotBe(entity2.RecordDate);
        }

        [Fact]
        public void BaseEntity_Properties_CanBeModifiedMultipleTimes()
        {
            // Arrange
            var entity = new BaseEntity { Id = 1, RecordDate = DateTime.Now };

            // Act
            entity.Id = 2;
            entity.Id = 3;
            entity.RecordDate = new DateTime(2025, 1, 1);
            entity.RecordDate = new DateTime(2025, 12, 31);

            // Assert
            entity.Id.Should().Be(3);
            entity.RecordDate.Should().Be(new DateTime(2025, 12, 31));
        }

        [Fact]
        public void BaseEntity_Id_MaxValue_ShouldWork()
        {
            // Arrange & Act
            var entity = new BaseEntity { Id = int.MaxValue };

            // Assert
            entity.Id.Should().Be(int.MaxValue);
            entity.Id.Should().Be(2147483647);
        }

        [Fact]
        public void BaseEntity_Id_MinValue_ShouldWork()
        {
            // Arrange & Act
            var entity = new BaseEntity { Id = int.MinValue };

            // Assert
            entity.Id.Should().Be(int.MinValue);
            entity.Id.Should().Be(-2147483648);
        }

        [Fact]
        public void BaseEntity_RecordDate_MinValue_ShouldWork()
        {
            // Arrange & Act
            var entity = new BaseEntity { RecordDate = DateTime.MinValue };

            // Assert
            entity.RecordDate.Should().Be(DateTime.MinValue);
            entity.RecordDate.Year.Should().Be(1);
        }

        [Fact]
        public void BaseEntity_RecordDate_MaxValue_ShouldWork()
        {
            // Arrange & Act
            var entity = new BaseEntity { RecordDate = DateTime.MaxValue };

            // Assert
            entity.RecordDate.Should().Be(DateTime.MaxValue);
            entity.RecordDate.Year.Should().Be(9999);
        }

        [Fact]
        public void BaseEntity_CanBeUsedAsBaseClass()
        {
            // Arrange & Act
            var kullanici = new Kullanıcı { Id = 1, RecordDate = DateTime.Now };
            var hasta = new Hasta { Id = 2, RecordDate = DateTime.Now };
            var doktor = new Doktor { Id = 3, RecordDate = DateTime.Now };

            // Assert
            kullanici.Should().BeAssignableTo<BaseEntity>();
            hasta.Should().BeAssignableTo<BaseEntity>();
            doktor.Should().BeAssignableTo<BaseEntity>();
        }

        [Fact]
        public void BaseEntity_InheritedClass_ShouldHaveBaseProperties()
        {
            // Arrange & Act
            var kullanici = new Kullanıcı
            {
                Id = 100,
                RecordDate = new DateTime(2025, 5, 15),
                İsim = "Test",
                Email = "test@test.com"
            };

            // Assert
            kullanici.Id.Should().Be(100);
            kullanici.RecordDate.Should().Be(new DateTime(2025, 5, 15));
            kullanici.İsim.Should().Be("Test");
        }

        [Fact]
        public void BaseEntity_RecordDate_PreservesTime()
        {
            // Arrange & Act
            var exactTime = new DateTime(2025, 11, 19, 14, 30, 45, 123);
            var entity = new BaseEntity { RecordDate = exactTime };

            // Assert
            entity.RecordDate.Hour.Should().Be(14);
            entity.RecordDate.Minute.Should().Be(30);
            entity.RecordDate.Second.Should().Be(45);
            entity.RecordDate.Millisecond.Should().Be(123);
        }

        [Fact]
        public void BaseEntity_RecordDate_WithDifferentKinds()
        {
            // Arrange & Act
            var utcDate = DateTime.UtcNow;
            var localDate = DateTime.Now;
            var unspecifiedDate = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Unspecified);

            var entity1 = new BaseEntity { RecordDate = utcDate };
            var entity2 = new BaseEntity { RecordDate = localDate };
            var entity3 = new BaseEntity { RecordDate = unspecifiedDate };

            // Assert
            entity1.RecordDate.Kind.Should().Be(DateTimeKind.Utc);
            entity2.RecordDate.Kind.Should().Be(DateTimeKind.Local);
            entity3.RecordDate.Kind.Should().Be(DateTimeKind.Unspecified);
        }

        [Fact]
        public void BaseEntity_CompareDates_ShouldWork()
        {
            // Arrange
            var date1 = new DateTime(2025, 1, 1);
            var date2 = new DateTime(2025, 12, 31);

            var entity1 = new BaseEntity { RecordDate = date1 };
            var entity2 = new BaseEntity { RecordDate = date2 };

            // Assert
            entity1.RecordDate.Should().BeBefore(entity2.RecordDate);
            entity2.RecordDate.Should().BeAfter(entity1.RecordDate);
        }
    }
}

