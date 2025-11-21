using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using FluentAssertions;
using ClinickService.Services;
using ClinickCore.Entities;
using ClinickCore.DTOs;
using ClinickDataAccess.Repository;

namespace Clinick.Tests.Services
{
    public class RandevuServiceTests
    {
        private readonly Mock<IGenericRepository<Doktor>> _mockDoktorRepo;
        private readonly Mock<IGenericRepository<Hasta>> _mockHastaRepo;
        private readonly Mock<IGenericRepository<Uzmanlık>> _mockUzmanlikRepo;
        private readonly Mock<IGenericRepository<Randevu>> _mockRandevuRepo;
        private readonly Mock<IGenericRepository<Kullanıcı>> _mockKullaniciRepo;
        private readonly RandevuService _service;

        public RandevuServiceTests()
        {
            _mockDoktorRepo = new Mock<IGenericRepository<Doktor>>();
            _mockHastaRepo = new Mock<IGenericRepository<Hasta>>();
            _mockUzmanlikRepo = new Mock<IGenericRepository<Uzmanlık>>();
            _mockRandevuRepo = new Mock<IGenericRepository<Randevu>>();
            _mockKullaniciRepo = new Mock<IGenericRepository<Kullanıcı>>();

            _service = new RandevuService(
                _mockDoktorRepo.Object,
                _mockHastaRepo.Object,
                _mockUzmanlikRepo.Object,
                _mockRandevuRepo.Object,
                _mockKullaniciRepo.Object
            );
        }

        [Fact]
        public void RandevuEkle_TarihGecmisse_HataDonmeli()
        {
            var dto = new RandevuOlusturDto { RandevuTarihi = DateTime.Now.AddDays(-1) };
            var result = _service.RandevuEkle(dto);
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("randevu alamazsınız");
        }

        [Fact]
        public void RandevuEkle_Cakisiyorsa_HataDonmeli()
        {
            // Arrange
            var tarih = DateTime.Now.AddDays(1);
            var dto = new RandevuOlusturDto { DoktorId = 1, HastaId = 1, RandevuTarihi = tarih };

            _mockHastaRepo.Setup(x => x.GetById(1)).Returns(new Hasta());
            _mockDoktorRepo.Setup(x => x.GetById(1)).Returns(new Doktor { UzmanlıkId = 5 });
            _mockUzmanlikRepo.Setup(x => x.GetById(5)).Returns(new Uzmanlık());

            // Çakışma simülasyonu
            _mockRandevuRepo.Setup(x => x.GetAll()).Returns(new List<Randevu>
            {
                new Randevu { DoktorId = 1, RandevuTarihi = tarih, Durum = "Beklemede" }
            }.AsQueryable());

            // Act
            var result = _service.RandevuEkle(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("randevusu bulunmaktadır");
        }

        [Fact]
        public void GecmisRandevularıTamamla_SadeceGecmisVeBekleyenleriGuncellemeli()
        {
            // Arrange
            var r1 = new Randevu { Id = 1, Durum = "Beklemede", RandevuTarihi = DateTime.Now.AddDays(-5) }; // Güncellenmeli
            var r2 = new Randevu { Id = 2, Durum = "İptal", RandevuTarihi = DateTime.Now.AddDays(-5) }; // Dokunulmamalı

            _mockRandevuRepo.Setup(x => x.GetAll()).Returns(new List<Randevu> { r1, r2 }.AsQueryable());

            // Act
            var result = _service.GeçmişRandevularıTamamla();

            // Assert
            result.IsSuccess.Should().BeTrue();
            _mockRandevuRepo.Verify(x => x.Update(It.Is<Randevu>(r => r.Id == 1 && r.Durum == "Tamamlandı")), Times.Once);
            _mockRandevuRepo.Verify(x => x.Update(It.Is<Randevu>(r => r.Id == 2)), Times.Never);
        }

        [Fact]
        public void GetMusaitRandevuSaatleri_DoluOlanlari_Getirmemeli()
        {
            // Arrange: Gelecek bir iş günü seç (Pazartesi vs)
            var tarih = DateTime.Now.AddDays(1);
            while (tarih.DayOfWeek == DayOfWeek.Saturday || tarih.DayOfWeek == DayOfWeek.Sunday)
                tarih = tarih.AddDays(1);

            // Saat 09:00 dolu olsun
            var doluSaat = new TimeSpan(9, 0, 0);
            var r = new Randevu
            {
                DoktorId = 1,
                RandevuTarihi = new DateTime(tarih.Year, tarih.Month, tarih.Day, 9, 0, 0),
                Durum = "Beklemede"
            };

            _mockRandevuRepo.Setup(x => x.GetAll()).Returns(new List<Randevu> { r }.AsQueryable());

            // Act
            var result = _service.GetMusaitRandevuSaatleri(1, tarih);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotContain("09:00"); // Dolu olduğu için listede olmamalı
            result.Data.Should().Contain("10:00"); // Boş olduğu için listede olmalı
        }
    }
}