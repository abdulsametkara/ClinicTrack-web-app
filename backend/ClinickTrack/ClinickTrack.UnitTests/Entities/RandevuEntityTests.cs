using Xunit;
using FluentAssertions;
using ClinickCore.Entities;
using System;

namespace ClinickTrack.Tests.Entities
{
    public class RandevuEntityTests
    {
        [Fact]
        public void Randevu_ShouldInheritFromBaseEntity()
        {
            // Arrange & Act
            var randevu = new Randevu();

            // Assert
            randevu.Should().BeAssignableTo<BaseEntity>();
        }

        [Fact]
        public void Randevu_ShouldSetProperties_Correctly()
        {
            // Arrange
            var randevuTarihi = new DateTime(2025, 12, 25, 14, 30, 0);

            // Act
            var randevu = new Randevu
            {
                Id = 1,
                RecordDate = DateTime.Now,
                HastaId = 10,
                DoktorId = 5,
                UzmanlıkId = 3,
                RandevuTarihi = randevuTarihi,
                Durum = "Bekliyor",
                HastaŞikayeti = "Baş ağrısı ve ateş",
                DoktorNotları = "Hastaya antibiyotik reçete edildi"
            };

            // Assert
            randevu.Id.Should().Be(1);
            randevu.HastaId.Should().Be(10);
            randevu.DoktorId.Should().Be(5);
            randevu.UzmanlıkId.Should().Be(3);
            randevu.RandevuTarihi.Should().Be(randevuTarihi);
            randevu.Durum.Should().Be("Bekliyor");
            randevu.HastaŞikayeti.Should().Be("Baş ağrısı ve ateş");
            randevu.DoktorNotları.Should().Be("Hastaya antibiyotik reçete edildi");
        }

        [Fact]
        public void Randevu_AllIds_ShouldBePositive()
        {
            // Arrange & Act
            var randevu = new Randevu
            {
                HastaId = 15,
                DoktorId = 8,
                UzmanlıkId = 2
            };

            // Assert
            randevu.HastaId.Should().BePositive();
            randevu.DoktorId.Should().BePositive();
            randevu.UzmanlıkId.Should().BePositive();
        }

        [Fact]
        public void Randevu_Durum_CommonStatuses()
        {
            // Test common appointment statuses
            var statuses = new[] { "Bekliyor", "Tamamlandı", "İptal Edildi", "Ertelendi", "Onaylandı" };

            foreach (var status in statuses)
            {
                var randevu = new Randevu { Durum = status };
                randevu.Durum.Should().Be(status);
            }
        }

        [Fact]
        public void Randevu_RandevuTarihi_FutureDate()
        {
            // Arrange & Act
            var futureDate = DateTime.Now.AddDays(7);
            var randevu = new Randevu { RandevuTarihi = futureDate };

            // Assert
            randevu.RandevuTarihi.Should().BeAfter(DateTime.Now);
        }

        [Fact]
        public void Randevu_RandevuTarihi_WithTime()
        {
            // Arrange & Act
            var appointmentTime = new DateTime(2025, 11, 20, 10, 30, 0);
            var randevu = new Randevu { RandevuTarihi = appointmentTime };

            // Assert
            randevu.RandevuTarihi.Hour.Should().Be(10);
            randevu.RandevuTarihi.Minute.Should().Be(30);
        }

        [Fact]
        public void Randevu_HastaŞikayeti_CanBeLong()
        {
            // Arrange
            var longComplaint = "Hasta son 3 gündür yüksek ateş, baş ağrısı, boğaz ağrısı ve halsizlik " +
                              "şikayetleriyle başvurdu. Önceden var olan hipertansiyon ve diyabet hastalıkları mevcut. " +
                              "İlaç kullanımına düzenli devam ediyor.";

            // Act
            var randevu = new Randevu { HastaŞikayeti = longComplaint };

            // Assert
            randevu.HastaŞikayeti.Should().Contain("ateş");
            randevu.HastaŞikayeti.Should().Contain("baş ağrısı");
            randevu.HastaŞikayeti.Length.Should().BeGreaterThan(100);
        }

        [Fact]
        public void Randevu_DoktorNotları_DetailedNotes()
        {
            // Arrange
            var detailedNotes = @"Muayene Bulguları:
- Ateş: 38.5°C
- Tansiyon: 120/80
- Nabız: 75/dk
Tanı: Akut farenjit
Tedavi: Antibiyotik reçete edildi (Amoksisilin 500mg 2x1)
Kontrol: 1 hafta sonra";

            // Act
            var randevu = new Randevu { DoktorNotları = detailedNotes };

            // Assert
            randevu.DoktorNotları.Should().Contain("Muayene");
            randevu.DoktorNotları.Should().Contain("Tanı");
            randevu.DoktorNotları.Should().Contain("Tedavi");
        }

        [Fact]
        public void Randevu_MultipleAppointments_SameDay()
        {
            // Arrange
            var baseDate = new DateTime(2025, 11, 25);

            // Act
            var morning = new Randevu { RandevuTarihi = baseDate.AddHours(9) };
            var afternoon = new Randevu { RandevuTarihi = baseDate.AddHours(14) };
            var evening = new Randevu { RandevuTarihi = baseDate.AddHours(17) };

            // Assert
            morning.RandevuTarihi.Date.Should().Be(afternoon.RandevuTarihi.Date);
            afternoon.RandevuTarihi.Date.Should().Be(evening.RandevuTarihi.Date);
            morning.RandevuTarihi.Hour.Should().BeLessThan(afternoon.RandevuTarihi.Hour);
        }

        [Fact]
        public void Randevu_EmptyComplaint_ShouldBeAllowed()
        {
            // Arrange & Act
            var randevu = new Randevu { HastaŞikayeti = "" };

            // Assert
            randevu.HastaŞikayeti.Should().BeEmpty();
        }

        [Fact]
        public void Randevu_EmptyNotes_ShouldBeAllowed()
        {
            // Arrange & Act
            var randevu = new Randevu { DoktorNotları = "" };

            // Assert
            randevu.DoktorNotları.Should().BeEmpty();
        }

        [Fact]
        public void Randevu_MultipleInstances_ShouldBeIndependent()
        {
            // Arrange & Act
            var randevu1 = new Randevu { Id = 1, HastaId = 10, DoktorId = 5 };
            var randevu2 = new Randevu { Id = 2, HastaId = 20, DoktorId = 8 };

            // Assert
            randevu1.Id.Should().NotBe(randevu2.Id);
            randevu1.HastaId.Should().NotBe(randevu2.HastaId);
            randevu1.DoktorId.Should().NotBe(randevu2.DoktorId);
        }

        [Fact]
        public void Randevu_RecordDate_ShouldBeAccessible()
        {
            // Arrange
            var recordDate = new DateTime(2025, 11, 19);

            // Act
            var randevu = new Randevu { RecordDate = recordDate };

            // Assert
            randevu.RecordDate.Should().Be(recordDate);
        }

        [Fact]
        public void Randevu_TurkishCharacters_InFields()
        {
            // Arrange & Act
            var randevu = new Randevu 
            { 
                HastaŞikayeti = "Baş ağrısı ve göğüs sıkışması",
                DoktorNotları = "Çocuk sağlığı kontrol muayenesi"
            };

            // Assert
            randevu.HastaŞikayeti.Should().Contain("ş");
            randevu.HastaŞikayeti.Should().Contain("ğ");
            randevu.DoktorNotları.Should().Contain("Ç");
            randevu.DoktorNotları.Should().Contain("ğ");
        }

        [Fact]
        public void Randevu_PastAppointment_ShouldBeValid()
        {
            // Arrange & Act
            var pastDate = DateTime.Now.AddDays(-7);
            var randevu = new Randevu 
            { 
                RandevuTarihi = pastDate,
                Durum = "Tamamlandı"
            };

            // Assert
            randevu.RandevuTarihi.Should().BeBefore(DateTime.Now);
            randevu.Durum.Should().Be("Tamamlandı");
        }

        [Fact]
        public void Randevu_CancelledAppointment_WithReason()
        {
            // Arrange & Act
            var randevu = new Randevu 
            { 
                Durum = "İptal Edildi",
                DoktorNotları = "Hasta tarafından iptal edildi - kişisel sebep"
            };

            // Assert
            randevu.Durum.Should().Be("İptal Edildi");
            randevu.DoktorNotları.Should().Contain("iptal");
        }
    }
}

