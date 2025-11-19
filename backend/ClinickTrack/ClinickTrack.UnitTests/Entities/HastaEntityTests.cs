using Xunit;
using FluentAssertions;
using ClinickCore.Entities;
using System;

namespace ClinickTrack.Tests.Entities
{
    public class HastaEntityTests
    {
        [Fact]
        public void Hasta_ShouldInheritFromBaseEntity()
        {
            // Arrange & Act
            var hasta = new Hasta();

            // Assert
            hasta.Should().BeAssignableTo<BaseEntity>();
        }

        [Fact]
        public void Hasta_ShouldSetProperties_Correctly()
        {
            // Arrange & Act
            var hasta = new Hasta
            {
                Id = 1,
                RecordDate = DateTime.Now,
                KullanıcıId = 10,
                Cinsiyet = "Erkek",
                KanGrubu = "A+",
                Adres = "İstanbul, Türkiye",
                AcilDurumKişisi = "Ali Yılmaz",
                AcilDurumTelefon = "5551234567"
            };

            // Assert
            hasta.Id.Should().Be(1);
            hasta.KullanıcıId.Should().Be(10);
            hasta.Cinsiyet.Should().Be("Erkek");
            hasta.KanGrubu.Should().Be("A+");
            hasta.Adres.Should().Be("İstanbul, Türkiye");
            hasta.AcilDurumKişisi.Should().Be("Ali Yılmaz");
            hasta.AcilDurumTelefon.Should().Be("5551234567");
        }

        [Fact]
        public void Hasta_AllOptionalFields_CanBeNull()
        {
            // Arrange & Act
            var hasta = new Hasta
            {
                KullanıcıId = 5,
                Cinsiyet = null,
                KanGrubu = null,
                Adres = null,
                AcilDurumKişisi = null,
                AcilDurumTelefon = null
            };

            // Assert
            hasta.KullanıcıId.Should().Be(5);
            hasta.Cinsiyet.Should().BeNull();
            hasta.KanGrubu.Should().BeNull();
            hasta.Adres.Should().BeNull();
            hasta.AcilDurumKişisi.Should().BeNull();
            hasta.AcilDurumTelefon.Should().BeNull();
        }

        [Fact]
        public void Hasta_KullanıcıId_ShouldBePositive()
        {
            // Arrange & Act
            var hasta = new Hasta { KullanıcıId = 100 };

            // Assert
            hasta.KullanıcıId.Should().BePositive();
            hasta.KullanıcıId.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Hasta_Cinsiyet_ValidValues()
        {
            // Test different gender values
            var erkek = new Hasta { Cinsiyet = "Erkek" };
            var kadin = new Hasta { Cinsiyet = "Kadın" };
            var diger = new Hasta { Cinsiyet = "Diğer" };

            erkek.Cinsiyet.Should().Be("Erkek");
            kadin.Cinsiyet.Should().Be("Kadın");
            diger.Cinsiyet.Should().Be("Diğer");
        }

        [Fact]
        public void Hasta_KanGrubu_AllBloodTypes()
        {
            // Test all blood types
            var bloodTypes = new[] { "A+", "A-", "B+", "B-", "AB+", "AB-", "0+", "0-" };

            foreach (var bloodType in bloodTypes)
            {
                var hasta = new Hasta { KanGrubu = bloodType };
                hasta.KanGrubu.Should().Be(bloodType);
            }
        }

        [Fact]
        public void Hasta_Adres_CanBeLong()
        {
            // Arrange
            var longAddress = "Atatürk Mahallesi, Cumhuriyet Caddesi No:123 Daire:4, " +
                            "Çankaya, Ankara, Türkiye, Posta Kodu: 06570";

            // Act
            var hasta = new Hasta { Adres = longAddress };

            // Assert
            hasta.Adres.Should().Be(longAddress);
            hasta.Adres.Length.Should().BeGreaterThan(50);
        }

        [Fact]
        public void Hasta_AcilDurumTelefon_TurkishFormat()
        {
            // Arrange & Act
            var hasta = new Hasta { AcilDurumTelefon = "5559876543" };

            // Assert
            hasta.AcilDurumTelefon.Should().StartWith("5");
            hasta.AcilDurumTelefon.Should().HaveLength(10);
        }

        [Fact]
        public void Hasta_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange & Act
            var hasta1 = new Hasta { Id = 1, KullanıcıId = 10 };
            var hasta2 = new Hasta { Id = 2, KullanıcıId = 20 };

            // Assert
            hasta1.Id.Should().NotBe(hasta2.Id);
            hasta1.KullanıcıId.Should().NotBe(hasta2.KullanıcıId);
        }

        [Fact]
        public void Hasta_RecordDate_ShouldBeAccessible()
        {
            // Arrange
            var recordDate = new DateTime(2025, 11, 19);

            // Act
            var hasta = new Hasta { RecordDate = recordDate };

            // Assert
            hasta.RecordDate.Should().Be(recordDate);
        }

        [Fact]
        public void Hasta_TurkishCharacters_InNames()
        {
            // Arrange & Act
            var hasta = new Hasta 
            { 
                AcilDurumKişisi = "Şükrü Özdemir" 
            };

            // Assert
            hasta.AcilDurumKişisi.Should().Contain("Ş");
            hasta.AcilDurumKişisi.Should().Contain("ü");
            hasta.AcilDurumKişisi.Should().Contain("Ö");
        }

        [Fact]
        public void Hasta_EmptyStrings_ShouldBeAllowed()
        {
            // Arrange & Act
            var hasta = new Hasta 
            { 
                Cinsiyet = "",
                KanGrubu = "",
                Adres = ""
            };

            // Assert
            hasta.Cinsiyet.Should().BeEmpty();
            hasta.KanGrubu.Should().BeEmpty();
            hasta.Adres.Should().BeEmpty();
        }

        [Fact]
        public void Hasta_KanGrubu_RareTypes()
        {
            // Test rare blood types
            var bombay = new Hasta { KanGrubu = "Bombay" };
            var duffy = new Hasta { KanGrubu = "Duffy Negative" };

            bombay.KanGrubu.Should().Be("Bombay");
            duffy.KanGrubu.Should().Be("Duffy Negative");
        }
    }
}

