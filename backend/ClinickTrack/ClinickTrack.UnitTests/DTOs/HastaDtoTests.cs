using Xunit;
using FluentAssertions;
using ClinickCore.DTOs;

namespace ClinickTrack.Tests.DTOs
{
    public class HastaDtoTests
    {
        [Fact]
        public void HastaOlusturDto_ShouldSetProperties_Correctly()
        {
            // Arrange & Act
            var dto = new HastaOlusturDto
            {
                KullanıcıId = 1,
                Cinsiyet = "Erkek",
                KanGrubu = "A+",
                Adres = "İstanbul, Türkiye",
                AcilDurumKişisi = "Ali Yılmaz",
                AcilDurumTelefon = "5551234567"
            };

            // Assert
            dto.KullanıcıId.Should().Be(1);
            dto.Cinsiyet.Should().Be("Erkek");
            dto.KanGrubu.Should().Be("A+");
            dto.Adres.Should().Be("İstanbul, Türkiye");
            dto.AcilDurumKişisi.Should().Be("Ali Yılmaz");
            dto.AcilDurumTelefon.Should().Be("5551234567");
        }

        [Fact]
        public void HastaOlusturDto_OptionalFields_CanBeNull()
        {
            // Arrange & Act
            var dto = new HastaOlusturDto
            {
                KullanıcıId = 1,
                Cinsiyet = null,
                KanGrubu = null,
                Adres = null,
                AcilDurumKişisi = null,
                AcilDurumTelefon = null
            };

            // Assert
            dto.KullanıcıId.Should().Be(1);
            dto.Cinsiyet.Should().BeNull();
            dto.KanGrubu.Should().BeNull();
            dto.Adres.Should().BeNull();
            dto.AcilDurumKişisi.Should().BeNull();
            dto.AcilDurumTelefon.Should().BeNull();
        }

        [Fact]
        public void HastaOlusturDto_WithMinimalData_ShouldWork()
        {
            // Arrange & Act
            var dto = new HastaOlusturDto
            {
                KullanıcıId = 5
            };

            // Assert
            dto.KullanıcıId.Should().Be(5);
            dto.Should().NotBeNull();
        }

        [Fact]
        public void HastaOlusturDto_KanGrubu_ValidValues()
        {
            // Test different blood types
            var bloodTypes = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "0+", "0-" };

            foreach (var bloodType in bloodTypes)
            {
                var dto = new HastaOlusturDto { KanGrubu = bloodType };
                dto.KanGrubu.Should().Be(bloodType);
            }
        }

        [Fact]
        public void HastaGüncelleDto_ShouldSetProperties_Correctly()
        {
            // Arrange & Act
            var dto = new HastaGüncelleDto
            {
                Cinsiyet = "Kadın",
                KanGrubu = "B-",
                Adres = "Ankara, Türkiye",
                AcilDurumKişisi = "Ayşe Demir",
                AcilDurumTelefon = "5559876543"
            };

            // Assert
            dto.Cinsiyet.Should().Be("Kadın");
            dto.KanGrubu.Should().Be("B-");
            dto.Adres.Should().Be("Ankara, Türkiye");
            dto.AcilDurumKişisi.Should().Be("Ayşe Demir");
            dto.AcilDurumTelefon.Should().Be("5559876543");
        }

        [Fact]
        public void HastaGüncelleDto_AllFields_CanBeNull()
        {
            // Arrange & Act
            var dto = new HastaGüncelleDto
            {
                Cinsiyet = null,
                KanGrubu = null,
                Adres = null,
                AcilDurumKişisi = null,
                AcilDurumTelefon = null
            };

            // Assert
            dto.Cinsiyet.Should().BeNull();
            dto.KanGrubu.Should().BeNull();
            dto.Adres.Should().BeNull();
            dto.AcilDurumKişisi.Should().BeNull();
            dto.AcilDurumTelefon.Should().BeNull();
        }

        [Fact]
        public void HastaGüncelleDto_PartialUpdate_ShouldWork()
        {
            // Arrange & Act - only update some fields
            var dto = new HastaGüncelleDto
            {
                Cinsiyet = "Erkek",
                KanGrubu = "A+",
                // Other fields remain null for partial update
            };

            // Assert
            dto.Cinsiyet.Should().NotBeNull();
            dto.KanGrubu.Should().NotBeNull();
            dto.Adres.Should().BeNull();
        }

        [Fact]
        public void HastaOlusturDto_Cinsiyet_CommonValues()
        {
            // Test common gender values
            var erkekDto = new HastaOlusturDto { Cinsiyet = "Erkek" };
            var kadınDto = new HastaOlusturDto { Cinsiyet = "Kadın" };
            var diğerDto = new HastaOlusturDto { Cinsiyet = "Diğer" };

            erkekDto.Cinsiyet.Should().Be("Erkek");
            kadınDto.Cinsiyet.Should().Be("Kadın");
            diğerDto.Cinsiyet.Should().Be("Diğer");
        }

        [Fact]
        public void HastaOlusturDto_LongAddress_ShouldWork()
        {
            // Arrange & Act
            var longAddress = "Atatürk Mahallesi, Cumhuriyet Caddesi No:123 Daire:4, Çankaya, Ankara, Türkiye, Posta Kodu: 06570";
            var dto = new HastaOlusturDto
            {
                KullanıcıId = 1,
                Adres = longAddress
            };

            // Assert
            dto.Adres.Should().Be(longAddress);
            dto.Adres.Should().Contain("Ankara");
        }
    }
}

