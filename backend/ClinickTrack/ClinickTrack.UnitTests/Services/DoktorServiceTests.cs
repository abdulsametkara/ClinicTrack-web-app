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
    public class DoktorServiceTests
    {
        private readonly Mock<IGenericRepository<Doktor>> _mockDoktorRepo;
        private readonly Mock<IGenericRepository<Kullanıcı>> _mockKullaniciRepo;
        private readonly Mock<IGenericRepository<Uzmanlık>> _mockUzmanlikRepo;
        private readonly Mock<IGenericRepository<Randevu>> _mockRandevuRepo;
        private readonly DoktorService _service;

        public DoktorServiceTests()
        {
            _mockDoktorRepo = new Mock<IGenericRepository<Doktor>>();
            _mockKullaniciRepo = new Mock<IGenericRepository<Kullanıcı>>();
            _mockUzmanlikRepo = new Mock<IGenericRepository<Uzmanlık>>();
            _mockRandevuRepo = new Mock<IGenericRepository<Randevu>>();

            _service = new DoktorService(
                _mockDoktorRepo.Object,
                _mockKullaniciRepo.Object,
                _mockUzmanlikRepo.Object,
                _mockRandevuRepo.Object
            );
        }

        [Fact]
        public void DoktorEkle_UzmanlikBulunamazsa_HataDonmeli()
        {
            // Arrange
            var dto = new DoktorOlusturDto { UzmanlıkId = 99 };
            _mockUzmanlikRepo.Setup(x => x.GetById(99)).Returns((Uzmanlık)null);

            // Act
            var result = _service.DoktorEkle(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("uzmanlık bulunamadı");
        }

        [Fact]
        public void DoktorEkle_EmailKullanimdaysa_HataDonmeli()
        {
            // Arrange
            var dto = new DoktorOlusturDto { UzmanlıkId = 1, Email = "test@test.com" };
            _mockUzmanlikRepo.Setup(x => x.GetById(1)).Returns(new Uzmanlık());

            // Email varmış gibi davranıyoruz
            _mockKullaniciRepo.Setup(x => x.GetAll()).Returns(new List<Kullanıcı>
            {
                new Kullanıcı { Email = "test@test.com" }
            }.AsQueryable());

            // Act
            var result = _service.DoktorEkle(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("email adresi zaten kullanılıyor");
        }

        [Fact]
        public void DoktorEkle_BasariliIse_KullaniciVeDoktorOlusturmalı()
        {
            // Arrange
            var dto = new DoktorOlusturDto
            {
                UzmanlıkId = 1,
                Email = "yeni@doktor.com",
                TelefonNumarası = "555",
                İsim = "Ali",
                Soyisim = "Veli"
            };

            _mockUzmanlikRepo.Setup(x => x.GetById(1)).Returns(new Uzmanlık());
            _mockKullaniciRepo.Setup(x => x.GetAll()).Returns(new List<Kullanıcı>().AsQueryable());

            // Act
            var result = _service.DoktorEkle(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            // Hem Kullanıcı hem Doktor tablosuna kayıt atılmalı
            _mockKullaniciRepo.Verify(x => x.Create(It.IsAny<Kullanıcı>()), Times.Once);
            _mockDoktorRepo.Verify(x => x.Create(It.IsAny<Doktor>()), Times.Once);
        }
    }
}