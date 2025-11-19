using Xunit;
using FluentAssertions;
using ClinickCore.DTOs;
using System;

namespace ClinickTrack.Tests.DTOs
{
    public class KullanıcıDtoTests
    {
        [Fact]
        public void KullanıcıOlusturDto_ShouldSetProperties_Correctly()
        {
            // Arrange & Act
            var dto = new KullanıcıOlusturDto
            {
                İsim = "Ahmet",
                Soyisim = "Yılmaz",
                TCNo = "12345678901",
                Email = "ahmet@test.com",
                Parola = "test123",
                Rol = "Hasta",
                DoğumTarihi = new DateTime(1990, 5, 15),
                UzmanlıkId = 1,
                TelefonNumarası = "5551234567"
            };

            // Assert
            dto.İsim.Should().Be("Ahmet");
            dto.Soyisim.Should().Be("Yılmaz");
            dto.TCNo.Should().Be("12345678901");
            dto.Email.Should().Be("ahmet@test.com");
            dto.Parola.Should().Be("test123");
            dto.Rol.Should().Be("Hasta");
            dto.DoğumTarihi.Should().Be(new DateTime(1990, 5, 15));
            dto.UzmanlıkId.Should().Be(1);
            dto.TelefonNumarası.Should().Be("5551234567");
        }

        [Fact]
        public void KullanıcıOlusturDto_UzmanlıkId_CanBeNull()
        {
            // Arrange & Act
            var dto = new KullanıcıOlusturDto
            {
                İsim = "Test",
                Soyisim = "User",
                UzmanlıkId = null
            };

            // Assert
            dto.UzmanlıkId.Should().BeNull();
        }

        [Fact]
        public void KullanıcıGirisDto_ShouldSetProperties_Correctly()
        {
            // Arrange & Act
            var dto = new KullanıcıGirisDto
            {
                Email = "test@test.com",
                Parola = "password123"
            };

            // Assert
            dto.Email.Should().Be("test@test.com");
            dto.Parola.Should().Be("password123");
        }

        [Fact]
        public void KullanıcıGirisDto_AllowsEmptyValues()
        {
            // Arrange & Act
            var dto = new KullanıcıGirisDto
            {
                Email = "",
                Parola = ""
            };

            // Assert
            dto.Email.Should().BeEmpty();
            dto.Parola.Should().BeEmpty();
        }

        [Fact]
        public void KullanıcıKayıtDto_ShouldSetProperties_Correctly()
        {
            // Arrange & Act
            var dto = new KullanıcıKayıtDto
            {
                İsim = "Mehmet",
                Soyisim = "Demir",
                TCNo = "98765432109",
                Email = "mehmet@test.com",
                Parola = "password456",
                DoğumTarihi = new DateTime(1985, 3, 20),
                Cinsiyet = "Erkek",
                TelefonNumarası = "5559876543"
            };

            // Assert
            dto.İsim.Should().Be("Mehmet");
            dto.Soyisim.Should().Be("Demir");
            dto.TCNo.Should().Be("98765432109");
            dto.Email.Should().Be("mehmet@test.com");
            dto.Parola.Should().Be("password456");
            dto.DoğumTarihi.Should().Be(new DateTime(1985, 3, 20));
            dto.Cinsiyet.Should().Be("Erkek");
            dto.TelefonNumarası.Should().Be("5559876543");
        }

        [Fact]
        public void KullanıcıGüncelleDto_ShouldSetProperties_Correctly()
        {
            // Arrange & Act
            var dto = new KullanıcıGüncelleDto
            {
                Email = "newemail@test.com",
                Parola = "newpassword"
            };

            // Assert
            dto.Email.Should().Be("newemail@test.com");
            dto.Parola.Should().Be("newpassword");
        }

        [Fact]
        public void ParolaGuncelleDto_ShouldSetProperties_Correctly()
        {
            // Arrange & Act
            var dto = new ParolaGuncelleDto
            {
                EskiParola = "oldpass123",
                YeniParola = "newpass456"
            };

            // Assert
            dto.EskiParola.Should().Be("oldpass123");
            dto.YeniParola.Should().Be("newpass456");
        }

        [Fact]
        public void LoginResponseDto_ShouldSetProperties_Correctly()
        {
            // Arrange
            var expirationDate = DateTime.Now.AddHours(1);

            // Act
            var dto = new LoginResponseDto
            {
                Token = "test-jwt-token",
                Expiration = expirationDate,
                KullanıcıId = 1,
                Email = "user@test.com",
                İsim = "Test",
                Soyisim = "User",
                Rol = "Admin"
            };

            // Assert
            dto.Token.Should().Be("test-jwt-token");
            dto.Expiration.Should().Be(expirationDate);
            dto.KullanıcıId.Should().Be(1);
            dto.Email.Should().Be("user@test.com");
            dto.İsim.Should().Be("Test");
            dto.Soyisim.Should().Be("User");
            dto.Rol.Should().Be("Admin");
        }

        [Fact]
        public void LoginResponseDto_RolValues_ShouldBeValid()
        {
            // Arrange & Act
            var adminDto = new LoginResponseDto { Rol = "Admin" };
            var doktorDto = new LoginResponseDto { Rol = "Doktor" };
            var hastaDto = new LoginResponseDto { Rol = "Hasta" };

            // Assert
            adminDto.Rol.Should().Be("Admin");
            doktorDto.Rol.Should().Be("Doktor");
            hastaDto.Rol.Should().Be("Hasta");
        }

        [Fact]
        public void KullanıcıOlusturDto_WithMinimalData_ShouldWork()
        {
            // Arrange & Act
            var dto = new KullanıcıOlusturDto
            {
                İsim = "A",
                Soyisim = "B",
                TCNo = "12345678901",
                Email = "a@b.c",
                Parola = "p",
                Rol = "Hasta"
            };

            // Assert
            dto.Should().NotBeNull();
            dto.İsim.Should().NotBeEmpty();
            dto.Email.Should().NotBeEmpty();
        }

        [Fact]
        public void KullanıcıKayıtDto_Cinsiyet_CanBeEmpty()
        {
            // Arrange & Act
            var dto = new KullanıcıKayıtDto
            {
                Cinsiyet = ""
            };

            // Assert
            dto.Cinsiyet.Should().BeEmpty();
        }
    }
}

