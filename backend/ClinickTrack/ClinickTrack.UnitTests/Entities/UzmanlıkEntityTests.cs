using Xunit;
using FluentAssertions;
using ClinickCore.Entities;
using System;

namespace ClinickTrack.Tests.Entities
{
    public class UzmanlıkEntityTests
    {
        [Fact]
        public void Uzmanlık_ShouldInheritFromBaseEntity()
        {
            // Arrange & Act
            var uzmanlik = new Uzmanlık();

            // Assert
            uzmanlik.Should().BeAssignableTo<BaseEntity>();
        }

        [Fact]
        public void Uzmanlık_ShouldSetProperties_Correctly()
        {
            // Arrange & Act
            var uzmanlik = new Uzmanlık
            {
                Id = 1,
                RecordDate = DateTime.Now,
                UzmanlıkAdı = "Kardiyoloji"
            };

            // Assert
            uzmanlik.Id.Should().Be(1);
            uzmanlik.UzmanlıkAdı.Should().Be("Kardiyoloji");
            uzmanlik.RecordDate.Should().BeCloseTo(DateTime.Now, TimeSpan.FromSeconds(1));
        }

        [Fact]
        public void Uzmanlık_MedicalSpecialties_ShouldWork()
        {
            // Test various medical specialties
            var specialties = new[]
            {
                "Kardiyoloji",
                "Nöroloji",
                "Ortopedi",
                "Pediatri",
                "Dermatoloji",
                "Göz Hastalıkları",
                "Kulak Burun Boğaz",
                "Genel Cerrahi",
                "İç Hastalıkları",
                "Kadın Hastalıkları ve Doğum"
            };

            foreach (var specialty in specialties)
            {
                var uzmanlik = new Uzmanlık { UzmanlıkAdı = specialty };
                uzmanlik.UzmanlıkAdı.Should().Be(specialty);
            }
        }

        [Fact]
        public void Uzmanlık_SurgicalSpecialties_ShouldWork()
        {
            // Test surgical specialties
            var surgicalSpecialties = new[]
            {
                "Genel Cerrahi",
                "Beyin ve Sinir Cerrahisi",
                "Kalp ve Damar Cerrahisi",
                "Plastik ve Rekonstrüktif Cerrahi",
                "Göğüs Cerrahisi"
            };

            foreach (var specialty in surgicalSpecialties)
            {
                var uzmanlik = new Uzmanlık { UzmanlıkAdı = specialty };
                uzmanlik.UzmanlıkAdı.Should().Contain("Cerrahi");
            }
        }

        [Fact]
        public void Uzmanlık_TurkishCharacters_ShouldWork()
        {
            // Arrange & Act
            var uzmanlik = new Uzmanlık 
            { 
                UzmanlıkAdı = "Çocuk Sağlığı ve Hastalıkları" 
            };

            // Assert
            uzmanlik.UzmanlıkAdı.Should().Contain("ğ");
            uzmanlik.UzmanlıkAdı.Should().Contain("ı");
            uzmanlik.UzmanlıkAdı.Should().Contain("Ç");
        }

        [Fact]
        public void Uzmanlık_LongName_ShouldWork()
        {
            // Arrange
            var longName = "Fiziksel Tıp ve Rehabilitasyon Uzmanlığı";

            // Act
            var uzmanlik = new Uzmanlık { UzmanlıkAdı = longName };

            // Assert
            uzmanlik.UzmanlıkAdı.Should().Be(longName);
            uzmanlik.UzmanlıkAdı.Length.Should().BeGreaterThan(20);
        }

        [Fact]
        public void Uzmanlık_WithSpecialCharacters_ShouldWork()
        {
            // Arrange & Act
            var uzmanlik = new Uzmanlık 
            { 
                UzmanlıkAdı = "Kulak-Burun-Boğaz (KBB)" 
            };

            // Assert
            uzmanlik.UzmanlıkAdı.Should().Contain("-");
            uzmanlik.UzmanlıkAdı.Should().Contain("(");
            uzmanlik.UzmanlıkAdı.Should().Contain(")");
        }

        [Fact]
        public void Uzmanlık_EmptyName_ShouldBeAllowed()
        {
            // Arrange & Act
            var uzmanlik = new Uzmanlık { UzmanlıkAdı = "" };

            // Assert
            uzmanlik.UzmanlıkAdı.Should().BeEmpty();
        }

        [Fact]
        public void Uzmanlık_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange & Act
            var kardiyoloji = new Uzmanlık { Id = 1, UzmanlıkAdı = "Kardiyoloji" };
            var noroloji = new Uzmanlık { Id = 2, UzmanlıkAdı = "Nöroloji" };
            var ortopedi = new Uzmanlık { Id = 3, UzmanlıkAdı = "Ortopedi" };

            // Assert
            kardiyoloji.Id.Should().NotBe(noroloji.Id);
            noroloji.Id.Should().NotBe(ortopedi.Id);
            kardiyoloji.UzmanlıkAdı.Should().NotBe(noroloji.UzmanlıkAdı);
        }

        [Fact]
        public void Uzmanlık_RecordDate_ShouldBeAccessible()
        {
            // Arrange
            var recordDate = new DateTime(2025, 11, 19);

            // Act
            var uzmanlik = new Uzmanlık { RecordDate = recordDate };

            // Assert
            uzmanlik.RecordDate.Should().Be(recordDate);
        }

        [Fact]
        public void Uzmanlık_InternalMedicineSpecialties()
        {
            // Test internal medicine sub-specialties
            var internalSpecialties = new[]
            {
                "İç Hastalıkları",
                "Endokrinoloji ve Metabolizma Hastalıkları",
                "Gastroenteroloji",
                "Hematoloji",
                "Nefroloji",
                "Onkoloji"
            };

            foreach (var specialty in internalSpecialties)
            {
                var uzmanlik = new Uzmanlık { UzmanlıkAdı = specialty };
                uzmanlik.UzmanlıkAdı.Should().NotBeNullOrEmpty();
            }
        }

        [Fact]
        public void Uzmanlık_Pediatrics_Subspecialties()
        {
            // Test pediatric sub-specialties
            var pediatricSpecialties = new[]
            {
                "Pediatri",
                "Çocuk Kardiyolojisi",
                "Çocuk Cerrahisi",
                "Çocuk Nörolojisi",
                "Çocuk Hematolojisi"
            };

            foreach (var specialty in pediatricSpecialties)
            {
                var uzmanlik = new Uzmanlık { UzmanlıkAdı = specialty };
                var containsChild = uzmanlik.UzmanlıkAdı.Contains("Çocuk");
                var isPediatri = uzmanlik.UzmanlıkAdı == "Pediatri";
                (containsChild || isPediatri).Should().BeTrue();
            }
        }

        [Fact]
        public void Uzmanlık_NullName_ShouldBeAllowed()
        {
            // Arrange & Act
            var uzmanlik = new Uzmanlık { UzmanlıkAdı = null! };

            // Assert
            uzmanlik.UzmanlıkAdı.Should().BeNull();
        }

        [Fact]
        public void Uzmanlık_CompareDifferentSpecialties()
        {
            // Arrange & Act
            var specialty1 = new Uzmanlık { UzmanlıkAdı = "Kardiyoloji" };
            var specialty2 = new Uzmanlık { UzmanlıkAdı = "Kardiyoloji" };
            var specialty3 = new Uzmanlık { UzmanlıkAdı = "Nöroloji" };

            // Assert
            specialty1.UzmanlıkAdı.Should().Be(specialty2.UzmanlıkAdı);
            specialty1.UzmanlıkAdı.Should().NotBe(specialty3.UzmanlıkAdı);
        }

        [Fact]
        public void Uzmanlık_WhitespaceInName_ShouldBePreserved()
        {
            // Arrange & Act
            var uzmanlik = new Uzmanlık 
            { 
                UzmanlıkAdı = "Kadın   Hastalıkları   ve   Doğum" 
            };

            // Assert
            uzmanlik.UzmanlıkAdı.Should().Contain("   "); // Multiple spaces
        }
    }
}

