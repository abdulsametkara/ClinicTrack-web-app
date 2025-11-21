using Xunit;
using FluentAssertions;
using ClinickCore.DTOs;
using System;

namespace ClinickTrack.Tests.DTOs
{
    public class RandevuDtoTests
    {
        [Fact]
        public void RandevuOlusturDto_ShouldSetProperties_Correctly()
        {
            // Arrange
            var randevuTarihi = new DateTime(2025, 12, 25, 14, 30, 0);

            // Act
            var dto = new RandevuOlusturDto
            {
                HastaId = 1,
                DoktorId = 5,
                RandevuTarihi = randevuTarihi,
                HastaŞikayeti = "Baş ağrısı ve ateş"
            };

            // Assert
            dto.HastaId.Should().Be(1);
            dto.DoktorId.Should().Be(5);
            dto.RandevuTarihi.Should().Be(randevuTarihi);
            dto.HastaŞikayeti.Should().Be("Baş ağrısı ve ateş");
        }

        [Fact]
        public void RandevuOlusturDto_FutureDate_ShouldWork()
        {
            // Arrange & Act
            var futureDate = DateTime.Now.AddDays(7);
            var dto = new RandevuOlusturDto
            {
                HastaId = 2,
                DoktorId = 3,
                RandevuTarihi = futureDate,
                HastaŞikayeti = "Kontrol"
            };

            // Assert
            dto.RandevuTarihi.Should().BeAfter(DateTime.Now);
            dto.RandevuTarihi.Date.Should().Be(futureDate.Date);
        }

        [Fact]
        public void RandevuOlusturDto_WithTime_ShouldPreserveTime()
        {
            // Arrange & Act
            var appointmentTime = new DateTime(2025, 11, 20, 10, 30, 0);
            var dto = new RandevuOlusturDto
            {
                RandevuTarihi = appointmentTime
            };

            // Assert
            dto.RandevuTarihi.Hour.Should().Be(10);
            dto.RandevuTarihi.Minute.Should().Be(30);
        }

        [Fact]
        public void RandevuOlusturDto_HastaŞikayeti_CanBeLong()
        {
            // Arrange & Act
            var longComplaint = "Hasta son 3 gündür yüksek ateş, baş ağrısı, boğaz ağrısı ve halsizlik şikayetleriyle başvurdu. " +
                              "Önceden var olan hipertansiyon ve diyabet hastalıkları mevcut.";
            var dto = new RandevuOlusturDto
            {
                HastaŞikayeti = longComplaint
            };

            // Assert
            dto.HastaŞikayeti.Should().Contain("ateş");
            dto.HastaŞikayeti.Should().Contain("baş ağrısı");
            dto.HastaŞikayeti.Length.Should().BeGreaterThan(50);
        }

        [Fact]
        public void RandevuOlusturDto_RequiredIds_ShouldBePositive()
        {
            // Arrange & Act
            var dto = new RandevuOlusturDto
            {
                HastaId = 10,
                DoktorId = 20
            };

            // Assert
            dto.HastaId.Should().BePositive();
            dto.DoktorId.Should().BePositive();
        }

        [Fact]
        public void RandevuGüncelleDto_ShouldSetProperties_Correctly()
        {
            // Arrange
            var yeniTarih = new DateTime(2025, 12, 30, 15, 0, 0);

            // Act
            var dto = new RandevuGüncelleDto
            {
                RandevuId = 1,
                RandevuTarihi = yeniTarih,
                HastaŞikayeti = "Güncel şikayet",
                Durum = "Tamamlandı",
                DoktorNotları = "Hasta muayene edildi. İlaç reçete edildi."
            };

            // Assert
            dto.RandevuId.Should().Be(1);
            dto.RandevuTarihi.Should().Be(yeniTarih);
            dto.HastaŞikayeti.Should().Be("Güncel şikayet");
            dto.Durum.Should().Be("Tamamlandı");
            dto.DoktorNotları.Should().Be("Hasta muayene edildi. İlaç reçete edildi.");
        }

        [Fact]
        public void RandevuGüncelleDto_OptionalFields_CanBeNull()
        {
            // Arrange & Act
            var dto = new RandevuGüncelleDto
            {
                RandevuId = 5,
                RandevuTarihi = null,
                HastaŞikayeti = null,
                Durum = null,
                DoktorNotları = null
            };

            // Assert
            dto.RandevuId.Should().Be(5);
            dto.RandevuTarihi.Should().BeNull();
            dto.HastaŞikayeti.Should().BeNull();
            dto.Durum.Should().BeNull();
            dto.DoktorNotları.Should().BeNull();
        }

        [Fact]
        public void RandevuGüncelleDto_PartialUpdate_OnlyDurum()
        {
            // Arrange & Act - only update status
            var dto = new RandevuGüncelleDto
            {
                RandevuId = 3,
                Durum = "İptal Edildi"
            };

            // Assert
            dto.RandevuId.Should().Be(3);
            dto.Durum.Should().Be("İptal Edildi");
            dto.RandevuTarihi.Should().BeNull();
            dto.DoktorNotları.Should().BeNull();
        }

        [Fact]
        public void RandevuGüncelleDto_Durum_CommonValues()
        {
            // Test common appointment statuses
            var statuses = new[] { "Bekliyor", "Tamamlandı", "İptal Edildi", "Ertelendi" };

            foreach (var status in statuses)
            {
                var dto = new RandevuGüncelleDto
                {
                    RandevuId = 1,
                    Durum = status
                };
                dto.Durum.Should().Be(status);
            }
        }

        [Fact]
        public void RandevuGüncelleDto_DoktorNotları_CanBeDetailed()
        {
            // Arrange & Act
            var detailedNotes = @"Muayene Bulguları:
- Ateş: 38.5°C
- Tansiyon: 120/80
- Nabız: 75/dk
Tanı: Akut farenjit
Tedavi: Antibiyotik reçete edildi (Amoksisilin 500mg 2x1)
Kontrol: 1 hafta sonra";

            var dto = new RandevuGüncelleDto
            {
                RandevuId = 1,
                DoktorNotları = detailedNotes
            };

            // Assert
            dto.DoktorNotları.Should().Contain("Muayene Bulguları");
            dto.DoktorNotları.Should().Contain("Tanı");
            dto.DoktorNotları.Should().Contain("Tedavi");
        }

        [Fact]
        public void RandevuOlusturDto_EmptyComplaint_ShouldWork()
        {
            // Arrange & Act
            var dto = new RandevuOlusturDto
            {
                HastaId = 1,
                DoktorId = 1,
                RandevuTarihi = DateTime.Now,
                HastaŞikayeti = ""
            };

            // Assert
            dto.HastaŞikayeti.Should().BeEmpty();
        }

        [Fact]
        public void RandevuGüncelleDto_OnlyUpdateTime_ShouldWork()
        {
            // Arrange & Act
            var newTime = new DateTime(2025, 12, 25, 16, 45, 0);
            var dto = new RandevuGüncelleDto
            {
                RandevuId = 7,
                RandevuTarihi = newTime
            };

            // Assert
            dto.RandevuTarihi.Should().NotBeNull();
            dto.RandevuTarihi.Value.Hour.Should().Be(16);
            dto.RandevuTarihi.Value.Minute.Should().Be(45);
        }

        [Fact]
        public void RandevuOlusturDto_MultipleAppointmentsSameDay_ShouldWork()
        {
            // Arrange
            var baseDate = new DateTime(2025, 11, 25);

            // Act
            var morning = new RandevuOlusturDto { RandevuTarihi = baseDate.AddHours(9) };
            var afternoon = new RandevuOlusturDto { RandevuTarihi = baseDate.AddHours(14) };
            var evening = new RandevuOlusturDto { RandevuTarihi = baseDate.AddHours(17) };

            // Assert
            morning.RandevuTarihi.Date.Should().Be(afternoon.RandevuTarihi.Date);
            afternoon.RandevuTarihi.Date.Should().Be(evening.RandevuTarihi.Date);
            morning.RandevuTarihi.Hour.Should().BeLessThan(afternoon.RandevuTarihi.Hour);
        }
    }
}

