using Xunit;
using FluentAssertions;
using ClinickCore.Entities;
using System;

namespace ClinickTrack.Tests.Entities
{
    public class DoktorEntityTests
    {
        [Fact]
        public void Doktor_ShouldInheritFromBaseEntity()
        {
            // Arrange & Act
            var doktor = new Doktor();

            // Assert
            doktor.Should().BeAssignableTo<BaseEntity>();
        }

        [Fact]
        public void Doktor_ShouldSetProperties_Correctly()
        {
            // Arrange
            var mezuniyetTarihi = new DateTime(2010, 6, 15);

            // Act
            var doktor = new Doktor
            {
                Id = 1,
                RecordDate = DateTime.Now,
                KullanıcıId = 10,
                UzmanlıkId = 5,
                DiplomaNo = "DIP12345",
                MezuniyetTarihi = mezuniyetTarihi,
                Ünvan = "Prof. Dr."
            };

            // Assert
            doktor.Id.Should().Be(1);
            doktor.KullanıcıId.Should().Be(10);
            doktor.UzmanlıkId.Should().Be(5);
            doktor.DiplomaNo.Should().Be("DIP12345");
            doktor.MezuniyetTarihi.Should().Be(mezuniyetTarihi);
            doktor.Ünvan.Should().Be("Prof. Dr.");
        }

        [Fact]
        public void Doktor_RequiredFields_ShouldNotBeNull()
        {
            // Arrange & Act
            var doktor = new Doktor
            {
                KullanıcıId = 15,
                UzmanlıkId = 3
            };

            // Assert
            doktor.KullanıcıId.Should().BePositive();
            doktor.UzmanlıkId.Should().BePositive();
        }

        [Fact]
        public void Doktor_OptionalFields_CanBeNull()
        {
            // Arrange & Act
            var doktor = new Doktor
            {
                KullanıcıId = 10,
                UzmanlıkId = 5,
                DiplomaNo = null,
                MezuniyetTarihi = null,
                Ünvan = null
            };

            // Assert
            doktor.DiplomaNo.Should().BeNull();
            doktor.MezuniyetTarihi.Should().BeNull();
            doktor.Ünvan.Should().BeNull();
        }

        [Fact]
        public void Doktor_Ünvan_CommonTitles()
        {
            // Test common academic titles
            var titles = new[] { "Dr.", "Doç. Dr.", "Prof. Dr.", "Op. Dr.", "Uzm. Dr." };

            foreach (var title in titles)
            {
                var doktor = new Doktor { Ünvan = title };
                doktor.Ünvan.Should().Be(title);
            }
        }

        [Fact]
        public void Doktor_MezuniyetTarihi_ShouldBePastDate()
        {
            // Arrange & Act
            var doktor = new Doktor 
            { 
                MezuniyetTarihi = new DateTime(2005, 7, 20) 
            };

            // Assert
            doktor.MezuniyetTarihi.Should().NotBeNull();
            doktor.MezuniyetTarihi.Value.Should().BeBefore(DateTime.Now);
        }

        [Fact]
        public void Doktor_DiplomaNo_AlphanumericFormat()
        {
            // Arrange & Act
            var doktor = new Doktor { DiplomaNo = "TR-MED-2010-12345" };

            // Assert
            doktor.DiplomaNo.Should().NotBeNullOrEmpty();
            doktor.DiplomaNo.Should().Contain("TR");
        }

        [Fact]
        public void Doktor_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange & Act
            var doktor1 = new Doktor { Id = 1, KullanıcıId = 10, UzmanlıkId = 1 };
            var doktor2 = new Doktor { Id = 2, KullanıcıId = 20, UzmanlıkId = 2 };

            // Assert
            doktor1.Id.Should().NotBe(doktor2.Id);
            doktor1.KullanıcıId.Should().NotBe(doktor2.KullanıcıId);
            doktor1.UzmanlıkId.Should().NotBe(doktor2.UzmanlıkId);
        }

        [Fact]
        public void Doktor_UzmanlıkId_ShouldBePositive()
        {
            // Arrange & Act
            var doktor = new Doktor { UzmanlıkId = 8 };

            // Assert
            doktor.UzmanlıkId.Should().BePositive();
            doktor.UzmanlıkId.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Doktor_RecordDate_ShouldBeAccessible()
        {
            // Arrange
            var recordDate = new DateTime(2025, 11, 19);

            // Act
            var doktor = new Doktor { RecordDate = recordDate };

            // Assert
            doktor.RecordDate.Should().Be(recordDate);
        }

        [Fact]
        public void Doktor_WithMultipleQualifications()
        {
            // Arrange & Act
            var doktor = new Doktor
            {
                Ünvan = "Prof. Dr.",
                DiplomaNo = "BOARD-CERT-2015-67890",
                MezuniyetTarihi = new DateTime(2000, 6, 15)
            };

            // Assert
            doktor.Ünvan.Should().Contain("Prof");
            doktor.DiplomaNo.Should().Contain("CERT");
            doktor.MezuniyetTarihi.Value.Year.Should().Be(2000);
        }

        [Fact]
        public void Doktor_EmptyStrings_ShouldBeAllowed()
        {
            // Arrange & Act
            var doktor = new Doktor 
            { 
                DiplomaNo = "",
                Ünvan = ""
            };

            // Assert
            doktor.DiplomaNo.Should().BeEmpty();
            doktor.Ünvan.Should().BeEmpty();
        }

        [Fact]
        public void Doktor_MezuniyetTarihi_RecentGraduate()
        {
            // Arrange & Act
            var recentDate = DateTime.Now.AddYears(-2);
            var doktor = new Doktor { MezuniyetTarihi = recentDate };

            // Assert
            doktor.MezuniyetTarihi.Should().NotBeNull();
            doktor.MezuniyetTarihi.Value.Should().BeAfter(DateTime.Now.AddYears(-5));
        }

        [Fact]
        public void Doktor_TurkishCharacters_InÜnvan()
        {
            // Arrange & Act
            var doktor = new Doktor { Ünvan = "Doç. Dr." };

            // Assert
            doktor.Ünvan.Should().Contain("ç");
        }
    }
}

