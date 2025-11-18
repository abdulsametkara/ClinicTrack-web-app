using Xunit;
using Moq;
using FluentAssertions;
using ClinickService.Services;
using ClinickDataAccess.Repository;
using ClinickCore.Entities;
using ClinickCore.DTOs;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ClinickTrack.Tests.Services
{
    public class KullanıcıServiceTests
    {
        private readonly Mock<IGenericRepository<Kullanıcı>> _mockRepo;
        private readonly Mock<IConfiguration> _mockConfig;
        private readonly KullanıcıService _service;

        public KullanıcıServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<Kullanıcı>>();
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
                .Returns("60");
            
            _service = new KullanıcıService(_mockRepo.Object, _mockConfig.Object);
        }

        [Fact]
        public void KullanıcıOlustur_ValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new KullanıcıOlusturDto
            {
                İsim = "Test",
                Soyisim = "User",
                TCNo = "12345678901",
                Email = "test@test.com",
                Parola = "test123",
                Rol = "Hasta"
            };

            var kullanıcılar = new List<Kullanıcı>().AsQueryable();
            _mockRepo.Setup(x => x.GetAll()).Returns(kullanıcılar);
            _mockRepo.Setup(x => x.Create(It.IsAny<Kullanıcı>()))
                .Callback<Kullanıcı>(k => k.Id = 1);

            // Act
            var result = _service.KullanıcıOlustur(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _mockRepo.Verify(x => x.Create(It.IsAny<Kullanıcı>()), Times.Once);
        }

        [Fact]
        public void KullanıcıOlustur_InvalidTCNo_ReturnsFailure()
        {
            // Arrange
            var dto = new KullanıcıOlusturDto
            {
                İsim = "Test",
                Soyisim = "User",
                TCNo = "123", // Geçersiz TC (11 haneli olmalı)
                Email = "test@test.com",
                Parola = "test123",
                Rol = "Hasta"
            };

            // Act
            var result = _service.KullanıcıOlustur(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("TC No");
        }

        [Fact]
        public void KullanıcıOlustur_InvalidRole_ReturnsFailure()
        {
            // Arrange
            var dto = new KullanıcıOlusturDto
            {
                İsim = "Test",
                Soyisim = "User",
                TCNo = "12345678901",
                Email = "test@test.com",
                Parola = "test123",
                Rol = "InvalidRole"
            };

            // Act
            var result = _service.KullanıcıOlustur(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("rol");
        }

        [Fact]
        public void KullanıcıOlustur_DuplicateEmail_ReturnsFailure()
        {
            // Arrange
            var dto = new KullanıcıOlusturDto
            {
                İsim = "Test",
                Soyisim = "User",
                TCNo = "12345678901",
                Email = "existing@test.com",
                Parola = "test123",
                Rol = "Hasta"
            };

            var existingUsers = new List<Kullanıcı>
            {
                new Kullanıcı
                {
                    Id = 1,
                    Email = "existing@test.com",
                    TCNo = "98765432109",
                    İsim = "Existing",
                    Soyisim = "User",
                    Parola = "hash",
                    Rol = "Hasta"
                }
            }.AsQueryable();

            _mockRepo.Setup(x => x.GetAll()).Returns(existingUsers);

            // Act
            var result = _service.KullanıcıOlustur(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("email");
        }
    }
}

