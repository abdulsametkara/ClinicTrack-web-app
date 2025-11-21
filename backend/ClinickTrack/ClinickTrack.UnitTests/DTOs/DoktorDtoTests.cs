using Xunit;
using FluentAssertions;
using ClinickCore.DTOs;
using System;

namespace ClinickTrack.Tests.DTOs
{
    public class DoktorDtoTests
    {
        [Fact]
        public void DoktorOlusturDto_ShouldSetProperties_Correctly()
        {
            // Arrange & Act
            var dto = new DoktorOlusturDto
            {
                İsim = "Dr. Ahmet",
                Soyisim = "Yılmaz",
                TCNo = "12345678901",
                UzmanlıkId = 1,
                TelefonNumarası = "5551234567",
                Email = "dr.ahmet@hastane.com",
                DiplomaNo = "DIP12345",
                MezuniyetTarihi = new DateTime(2010, 6, 15),
                Ünvan = "Doç. Dr."
            };

            // Assert
            dto.İsim.Should().Be("Dr. Ahmet");
            dto.Soyisim.Should().Be("Yılmaz");
            dto.TCNo.Should().Be("12345678901");
            dto.UzmanlıkId.Should().Be(1);
            dto.TelefonNumarası.Should().Be("5551234567");
            dto.Email.Should().Be("dr.ahmet@hastane.com");
            dto.DiplomaNo.Should().Be("DIP12345");
            dto.MezuniyetTarihi.Should().Be(new DateTime(2010, 6, 15));
            dto.Ünvan.Should().Be("Doç. Dr.");
        }

        [Fact]
        public void DoktorOlusturDto_RequiredFields_ShouldNotBeNull()
        {
            // Arrange & Act
            var dto = new DoktorOlusturDto
            {
                İsim = "Mehmet",
                Soyisim = "Demir",
                TCNo = "98765432109",
                UzmanlıkId = 2,
                TelefonNumarası = "5559876543",
                Email = "mehmet@clinic.com"
            };

            // Assert
            dto.İsim.Should().NotBeNullOrEmpty();
            dto.Soyisim.Should().NotBeNullOrEmpty();
            dto.TCNo.Should().NotBeNullOrEmpty();
            dto.UzmanlıkId.Should().BeGreaterThan(0);
            dto.Email.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public void DoktorOlusturDto_OptionalFields_CanBeNull()
        {
            // Arrange & Act
            var dto = new DoktorOlusturDto
            {
                İsim = "Test",
                Soyisim = "Doktor",
                TCNo = "11111111111",
                UzmanlıkId = 1,
                TelefonNumarası = "5551111111",
                Email = "test@test.com",
                DiplomaNo = null,
                MezuniyetTarihi = null,
                Ünvan = null
            };

            // Assert
            dto.DiplomaNo.Should().BeNull();
            dto.MezuniyetTarihi.Should().BeNull();
            dto.Ünvan.Should().BeNull();
        }

        [Fact]
        public void DoktorOlusturDto_Ünvan_CommonValues()
        {
            // Test common academic titles
            var titles = new[] { "Dr.", "Doç. Dr.", "Prof. Dr.", "Op. Dr.", "Uzm. Dr." };

            foreach (var title in titles)
            {
                var dto = new DoktorOlusturDto
                {
                    Ünvan = title,
                    İsim = "Test",
                    Soyisim = "Doctor"
                };
                dto.Ünvan.Should().Be(title);
            }
        }

        [Fact]
        public void DoktorOlusturDto_MezuniyetTarihi_PastDate()
        {
            // Arrange & Act
            var graduationDate = new DateTime(2005, 7, 20);
            var dto = new DoktorOlusturDto
            {
                MezuniyetTarihi = graduationDate
            };

            // Assert
            dto.MezuniyetTarihi.Should().Be(graduationDate);
            dto.MezuniyetTarihi.Should().BeBefore(DateTime.Now);
        }

        [Fact]
        public void DoktorGüncelleDto_ShouldSetProperties_Correctly()
        {
            // Arrange & Act
            var dto = new DoktorGüncelleDto
            {
                UzmanlıkId = 3,
                TelefonNumarası = "5552223344",
                Email = "updated@email.com",
                DiplomaNo = "NEWDIP789",
                MezuniyetTarihi = new DateTime(2015, 8, 10),
                Ünvan = "Prof. Dr."
            };

            // Assert
            dto.UzmanlıkId.Should().Be(3);
            dto.TelefonNumarası.Should().Be("5552223344");
            dto.Email.Should().Be("updated@email.com");
            dto.DiplomaNo.Should().Be("NEWDIP789");
            dto.MezuniyetTarihi.Should().Be(new DateTime(2015, 8, 10));
            dto.Ünvan.Should().Be("Prof. Dr.");
        }

        [Fact]
        public void DoktorGüncelleDto_AllFields_CanBeNull()
        {
            // Arrange & Act
            var dto = new DoktorGüncelleDto
            {
                UzmanlıkId = null,
                TelefonNumarası = null,
                Email = null,
                DiplomaNo = null,
                MezuniyetTarihi = null,
                Ünvan = null
            };

            // Assert
            dto.UzmanlıkId.Should().BeNull();
            dto.TelefonNumarası.Should().BeNull();
            dto.Email.Should().BeNull();
            dto.DiplomaNo.Should().BeNull();
            dto.MezuniyetTarihi.Should().BeNull();
            dto.Ünvan.Should().BeNull();
        }

        [Fact]
        public void DoktorGüncelleDto_PartialUpdate_ShouldWork()
        {
            // Arrange & Act - only update email and phone
            var dto = new DoktorGüncelleDto
            {
                Email = "newemail@test.com",
                TelefonNumarası = "5559999999"
                // Other fields remain null for partial update
            };

            // Assert
            dto.Email.Should().NotBeNull();
            dto.TelefonNumarası.Should().NotBeNull();
            dto.UzmanlıkId.Should().BeNull();
            dto.DiplomaNo.Should().BeNull();
        }

        [Fact]
        public void DoktorOlusturDto_TCNo_ShouldBe11Digits()
        {
            // Arrange & Act
            var dto = new DoktorOlusturDto
            {
                TCNo = "12345678901" // 11 digits
            };

            // Assert
            dto.TCNo.Should().HaveLength(11);
            dto.TCNo.Should().MatchRegex("^[0-9]{11}$");
        }

        [Fact]
        public void DoktorOlusturDto_Email_ValidFormat()
        {
            // Arrange & Act
            var dto = new DoktorOlusturDto
            {
                Email = "doctor@hospital.com"
            };

            // Assert
            dto.Email.Should().Contain("@");
            dto.Email.Should().Contain(".");
        }

        [Fact]
        public void DoktorGüncelleDto_OnlyÜnvan_Update()
        {
            // Arrange & Act
            var dto = new DoktorGüncelleDto
            {
                Ünvan = "Doç. Dr."
            };

            // Assert
            dto.Ünvan.Should().Be("Doç. Dr.");
            dto.UzmanlıkId.Should().BeNull();
            dto.Email.Should().BeNull();
        }
    }
}

