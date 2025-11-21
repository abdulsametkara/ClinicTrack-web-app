using Xunit;
using FluentAssertions;
using ClinickCore.DTOs;

namespace ClinickTrack.Tests.DTOs
{
    public class UzmanlıkDtoTests
    {
        [Fact]
        public void UzmanlıkOlusturDto_ShouldSetProperty_Correctly()
        {
            // Arrange & Act
            var dto = new UzmanlıkOlusturDto
            {
                UzmanlıkAdı = "Kardiyoloji"
            };

            // Assert
            dto.UzmanlıkAdı.Should().Be("Kardiyoloji");
        }

        [Fact]
        public void UzmanlıkOlusturDto_MedicalSpecialties_ShouldWork()
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
                // Act
                var dto = new UzmanlıkOlusturDto { UzmanlıkAdı = specialty };

                // Assert
                dto.UzmanlıkAdı.Should().Be(specialty);
                dto.UzmanlıkAdı.Should().NotBeNullOrEmpty();
            }
        }

        [Fact]
        public void UzmanlıkOlusturDto_EmptyName_ShouldBeAllowed()
        {
            // Arrange & Act
            var dto = new UzmanlıkOlusturDto
            {
                UzmanlıkAdı = ""
            };

            // Assert
            dto.UzmanlıkAdı.Should().BeEmpty();
        }

        [Fact]
        public void UzmanlıkOlusturDto_LongName_ShouldWork()
        {
            // Arrange & Act
            var longName = "Fiziksel Tıp ve Rehabilitasyon Uzmanlığı";
            var dto = new UzmanlıkOlusturDto
            {
                UzmanlıkAdı = longName
            };

            // Assert
            dto.UzmanlıkAdı.Should().Be(longName);
            dto.UzmanlıkAdı.Length.Should().BeGreaterThan(20);
        }

        [Fact]
        public void UzmanlıkOlusturDto_WithSpecialCharacters_ShouldWork()
        {
            // Arrange & Act
            var dto = new UzmanlıkOlusturDto
            {
                UzmanlıkAdı = "Kulak-Burun-Boğaz (KBB)"
            };

            // Assert
            dto.UzmanlıkAdı.Should().Contain("-");
            dto.UzmanlıkAdı.Should().Contain("(");
            dto.UzmanlıkAdı.Should().Contain(")");
        }

        [Fact]
        public void UzmanlıkGüncelleDto_ShouldSetProperties_Correctly()
        {
            // Arrange & Act
            var dto = new UzmanlıkGüncelleDto
            {
                UzmanlıkId = 1,
                UzmanlıkAdı = "Göğüs Hastalıkları"
            };

            // Assert
            dto.UzmanlıkId.Should().Be(1);
            dto.UzmanlıkAdı.Should().Be("Göğüs Hastalıkları");
        }

        [Fact]
        public void UzmanlıkGüncelleDto_IdShouldBePositive()
        {
            // Arrange & Act
            var dto = new UzmanlıkGüncelleDto
            {
                UzmanlıkId = 5,
                UzmanlıkAdı = "Test"
            };

            // Assert
            dto.UzmanlıkId.Should().BePositive();
            dto.UzmanlıkId.Should().BeGreaterThan(0);
        }

        [Fact]
        public void UzmanlıkGüncelleDto_UpdateName_ShouldWork()
        {
            // Arrange & Act
            var dto = new UzmanlıkGüncelleDto
            {
                UzmanlıkId = 3,
                UzmanlıkAdı = "Güncellenen Uzmanlık Adı"
            };

            // Assert
            dto.UzmanlıkId.Should().Be(3);
            dto.UzmanlıkAdı.Should().Be("Güncellenen Uzmanlık Adı");
        }

        [Fact]
        public void UzmanlıkOlusturDto_TurkishCharacters_ShouldWork()
        {
            // Arrange & Act - Test Turkish characters
            var dto = new UzmanlıkOlusturDto
            {
                UzmanlıkAdı = "Çocuk Sağlığı ve Hastalıkları"
            };

            // Assert
            dto.UzmanlıkAdı.Should().Contain("ğ");
            dto.UzmanlıkAdı.Should().Contain("ı");
            dto.UzmanlıkAdı.Should().Contain("Ç");
        }

        [Fact]
        public void UzmanlıkGüncelleDto_DifferentIds_ShouldBeUnique()
        {
            // Arrange & Act
            var dto1 = new UzmanlıkGüncelleDto { UzmanlıkId = 1, UzmanlıkAdı = "Kardiyoloji" };
            var dto2 = new UzmanlıkGüncelleDto { UzmanlıkId = 2, UzmanlıkAdı = "Nöroloji" };
            var dto3 = new UzmanlıkGüncelleDto { UzmanlıkId = 3, UzmanlıkAdı = "Ortopedi" };

            // Assert
            dto1.UzmanlıkId.Should().NotBe(dto2.UzmanlıkId);
            dto2.UzmanlıkId.Should().NotBe(dto3.UzmanlıkId);
            dto1.UzmanlıkId.Should().NotBe(dto3.UzmanlıkId);
        }

        [Fact]
        public void UzmanlıkOlusturDto_SurgicalSpecialties_ShouldWork()
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
                var dto = new UzmanlıkOlusturDto { UzmanlıkAdı = specialty };
                dto.UzmanlıkAdı.Should().Contain("Cerrahi");
            }
        }

        [Fact]
        public void UzmanlıkGüncelleDto_ZeroId_ShouldBeAllowed()
        {
            // Arrange & Act (although business logic might reject this)
            var dto = new UzmanlıkGüncelleDto
            {
                UzmanlıkId = 0,
                UzmanlıkAdı = "Test"
            };

            // Assert
            dto.UzmanlıkId.Should().Be(0);
        }

        [Fact]
        public void UzmanlıkOlusturDto_CompareNames_ShouldWork()
        {
            // Arrange & Act
            var dto1 = new UzmanlıkOlusturDto { UzmanlıkAdı = "Kardiyoloji" };
            var dto2 = new UzmanlıkOlusturDto { UzmanlıkAdı = "Kardiyoloji" };
            var dto3 = new UzmanlıkOlusturDto { UzmanlıkAdı = "Nöroloji" };

            // Assert
            dto1.UzmanlıkAdı.Should().Be(dto2.UzmanlıkAdı);
            dto1.UzmanlıkAdı.Should().NotBe(dto3.UzmanlıkAdı);
        }
    }
}

