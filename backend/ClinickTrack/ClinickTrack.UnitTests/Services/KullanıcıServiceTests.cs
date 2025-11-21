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
using Microsoft.Extensions.Configuration;

namespace Clinick.Tests.Services
{
    public class KullanıcıServiceTests
    {
        private readonly Mock<IGenericRepository<Kullanıcı>> _mockKullaniciRepo;
        private readonly Mock<IGenericRepository<Hasta>> _mockHastaRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly KullanıcıService _service;

        public KullanıcıServiceTests()
        {
            _mockKullaniciRepo = new Mock<IGenericRepository<Kullanıcı>>();
            _mockHastaRepo = new Mock<IGenericRepository<Hasta>>();
            _mockConfig = new Mock<IConfiguration>();
            
            // appsettings değerlerini mock'la
            _mockConfig.Setup(x => x["Security:PasswordHashSecretKey"])
                .Returns("test-secret-key-for-testing-purposes-12345");
            _mockConfig.Setup(x => x["JwtSettings:Key"])
                .Returns("super-secret-jwt-key-minimum-32-characters-long-for-testing");
            _mockConfig.Setup(x => x["JwtSettings:Issuer"])
                .Returns("ClinickTrackApi");
            _mockConfig.Setup(x => x["JwtSettings:Audience"])
                .Returns("ClinickTrackClient");
            _mockConfig.Setup(x => x["JwtSettings:ExpiryInMinutes"])
                .Returns("6 0");
            
            _service = new KullanıcıService(_mockRepo.Object, _mockHastaRepo.Object, _mockConfig.Object);
        }

        [Fact]
        public void KullanıcıOlustur_TCNoHataliysa_HataDonmeli()
        {
            var dto = new KullanıcıOlusturDto
            {
                Email = "a@b.com",
                Rol = "Hasta",
                İsim = "Test",
                Soyisim = "User",
                Parola = "123456",
                TCNo = "123" // 11 hane değil
            };
            var result = _service.KullanıcıOlustur(dto);
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("11 haneli");
        }

        [Fact]
        public void KullanıcıOlustur_RolHastaIse_HastaTablosunaEklemeli()
        {
            // Arrange
            var dto = new KullanıcıOlusturDto
            {
                İsim = "Test",
                Soyisim = "User",
                TCNo = "11111111111",
                Email = "t@t.com",
                Rol = "Hasta",
                Parola = "123"
            };
            _mockKullaniciRepo.Setup(x => x.GetAll()).Returns(new List<Kullanıcı>().AsQueryable());

            // Act
            var result = _service.KullanıcıOlustur(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _mockKullaniciRepo.Verify(x => x.Create(It.IsAny<Kullanıcı>()), Times.Once);
            _mockHastaRepo.Verify(x => x.Create(It.IsAny<Hasta>()), Times.Once); // Hasta tablosuna da gitmeli
        }

        [Fact]
        public void Login_ParolaYanlissa_HataDonmeli()
        {
            // Arrange
            // Servis içinde HashPassword yapılıyor. 
            // Biz manuel olarak "dogruParola" + "secret_key" hash'ini simüle etmeliyiz ama
            // basitçe Mock setup'ında GetAll() dönerken veritabanındaki parolayı,
            // servisin üreteceği hash ile eşleşmeyecek bir şey verelim.

            var dbUser = new Kullanıcı { Email = "a@b.com", Parola = "HASHED_REAL_PASSWORD" };
            _mockKullaniciRepo.Setup(x => x.GetAll()).Returns(new List<Kullanıcı> { dbUser }.AsQueryable());

            var loginDto = new KullanıcıGirisDto { Email = "a@b.com", Parola = "yanlisParola" };

            // Act
            var result = _service.Login(loginDto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("hatalı");
        }
    }
}