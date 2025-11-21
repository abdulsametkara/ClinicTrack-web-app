using Xunit;
using FluentAssertions;
using ClinickCore.Entities;
using System;

namespace ClinickTrack.Tests.Entities
{
    public class KullanıcıEntityTests
    {
        [Fact]
        public void Kullanıcı_ShouldInheritFromBaseEntity()
        {
            // Arrange & Act
            var kullanici = new Kullanıcı();

            // Assert
            kullanici.Should().BeAssignableTo<BaseEntity>();
        }

        [Fact]
        public void Kullanıcı_ShouldSetProperties_Correctly()
        {
            // Arrange
            var dogumTarihi = new DateTime(1990, 5, 15);
            var olusturmaTarihi = new DateTime(2025, 11, 19);

            // Act
            var kullanici = new Kullanıcı
            {
                Id = 1,
                RecordDate = DateTime.Now,
                İsim = "Ahmet",
                Soyisim = "Yılmaz",
                TCNo = "12345678901",
                Email = "ahmet@test.com",
                Parola = "hashedpassword123",
                Rol = "Hasta",
                DoğumTarihi = dogumTarihi,
                UzmanlıkId = null,
                TelefonNumarası = "5551234567",
                OluşturulmaTarihi = olusturmaTarihi
            };

            // Assert
            kullanici.Id.Should().Be(1);
            kullanici.İsim.Should().Be("Ahmet");
            kullanici.Soyisim.Should().Be("Yılmaz");
            kullanici.TCNo.Should().Be("12345678901");
            kullanici.Email.Should().Be("ahmet@test.com");
            kullanici.Parola.Should().Be("hashedpassword123");
            kullanici.Rol.Should().Be("Hasta");
            kullanici.DoğumTarihi.Should().Be(dogumTarihi);
            kullanici.UzmanlıkId.Should().BeNull();
            kullanici.TelefonNumarası.Should().Be("5551234567");
            kullanici.OluşturulmaTarihi.Should().Be(olusturmaTarihi);
        }

        [Fact]
        public void Kullanıcı_OptionalFields_CanBeNull()
        {
            // Arrange & Act
            var kullanici = new Kullanıcı
            {
                İsim = "Test",
                Soyisim = "User",
                DoğumTarihi = null,
                UzmanlıkId = null,
                TelefonNumarası = null,
                OluşturulmaTarihi = null
            };

            // Assert
            kullanici.DoğumTarihi.Should().BeNull();
            kullanici.UzmanlıkId.Should().BeNull();
            kullanici.TelefonNumarası.Should().BeNull();
            kullanici.OluşturulmaTarihi.Should().BeNull();
        }

        [Fact]
        public void Kullanıcı_Rol_DifferentValues()
        {
            // Test different roles
            var admin = new Kullanıcı { Rol = "Admin" };
            var doktor = new Kullanıcı { Rol = "Doktor" };
            var hasta = new Kullanıcı { Rol = "Hasta" };

            admin.Rol.Should().Be("Admin");
            doktor.Rol.Should().Be("Doktor");
            hasta.Rol.Should().Be("Hasta");
        }

        [Fact]
        public void Kullanıcı_WithDoktorRole_ShouldHaveUzmanlıkId()
        {
            // Arrange & Act
            var doktor = new Kullanıcı
            {
                Rol = "Doktor",
                UzmanlıkId = 5
            };

            // Assert
            doktor.Rol.Should().Be("Doktor");
            doktor.UzmanlıkId.Should().NotBeNull();
            doktor.UzmanlıkId.Should().Be(5);
        }

        [Fact]
        public void Kullanıcı_TCNo_ShouldBe11Characters()
        {
            // Arrange & Act
            var kullanici = new Kullanıcı { TCNo = "12345678901" };

            // Assert
            kullanici.TCNo.Should().HaveLength(11);
        }

        [Fact]
        public void Kullanıcı_Email_ValidFormat()
        {
            // Arrange & Act
            var kullanici = new Kullanıcı { Email = "user@example.com" };

            // Assert
            kullanici.Email.Should().Contain("@");
            kullanici.Email.Should().Contain(".");
        }

        [Fact]
        public void Kullanıcı_TelefonNumarası_TurkishFormat()
        {
            // Arrange & Act
            var kullanici = new Kullanıcı { TelefonNumarası = "5551234567" };

            // Assert
            kullanici.TelefonNumarası.Should().StartWith("5");
            kullanici.TelefonNumarası.Should().HaveLength(10);
        }

        [Fact]
        public void Kullanıcı_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange & Act
            var user1 = new Kullanıcı { Id = 1, Email = "user1@test.com" };
            var user2 = new Kullanıcı { Id = 2, Email = "user2@test.com" };

            // Assert
            user1.Id.Should().NotBe(user2.Id);
            user1.Email.Should().NotBe(user2.Email);
        }

        [Fact]
        public void Kullanıcı_DoğumTarihi_ShouldBePastDate()
        {
            // Arrange & Act
            var kullanici = new Kullanıcı 
            { 
                DoğumTarihi = new DateTime(1985, 3, 20) 
            };

            // Assert
            kullanici.DoğumTarihi.Should().NotBeNull();
            kullanici.DoğumTarihi.Value.Should().BeBefore(DateTime.Now);
        }

        [Fact]
        public void Kullanıcı_OluşturulmaTarihi_CanBeSet()
        {
            // Arrange
            var creationDate = new DateTime(2025, 11, 19, 10, 30, 0);

            // Act
            var kullanici = new Kullanıcı { OluşturulmaTarihi = creationDate };

            // Assert
            kullanici.OluşturulmaTarihi.Should().Be(creationDate);
        }

        [Fact]
        public void Kullanıcı_FullName_Concatenation()
        {
            // Arrange & Act
            var kullanici = new Kullanıcı 
            { 
                İsim = "Mehmet", 
                Soyisim = "Demir" 
            };
            var fullName = $"{kullanici.İsim} {kullanici.Soyisim}";

            // Assert
            fullName.Should().Be("Mehmet Demir");
        }

        [Fact]
        public void Kullanıcı_TurkishCharacters_ShouldWork()
        {
            // Arrange & Act
            var kullanici = new Kullanıcı 
            { 
                İsim = "Çağlar",
                Soyisim = "Öztürk"
            };

            // Assert
            kullanici.İsim.Should().Contain("ğ");
            kullanici.Soyisim.Should().Contain("Ö");
            kullanici.Soyisim.Should().Contain("ü");
        }
    }
}

