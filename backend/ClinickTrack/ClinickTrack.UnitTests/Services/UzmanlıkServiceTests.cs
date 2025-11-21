using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Moq;
using FluentAssertions;
using ClinickService.Services;
using ClinickCore.Entities;
using ClinickDataAccess.Repository;

namespace Clinick.Tests.Services
{
    public class UzmanlıkServiceTests
    {
        private readonly Mock<IGenericRepository<Uzmanlık>> _mockRepo;
        private readonly UzmanlıkService _service;

        public UzmanlıkServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<Uzmanlık>>();
            _service = new UzmanlıkService(_mockRepo.Object);
        }

        [Fact]
        public void UzmanlıkEkle_IsimBos_HataDonmeli()
        {
            var result = _service.UzmanlıkEkle("");
            result.IsSuccess.Should().BeFalse();
        }

        [Fact]
        public void UzmanlıkEkle_AyniIsimVarsa_HataDonmeli()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetAll()).Returns(new List<Uzmanlık>
            {
                new Uzmanlık { UzmanlıkAdı = "Kardiyoloji" }
            }.AsQueryable());

            // Act - Küçük harf büyük harf duyarlılığı testi (code'da ToLower var)
            var result = _service.UzmanlıkEkle("kardiyoloji");

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("zaten mevcut");
        }

        [Fact]
        public void UzmanlıkEkle_Basarili_Kaydetmeli()
        {
            // Arrange
            _mockRepo.Setup(x => x.GetAll()).Returns(new List<Uzmanlık>().AsQueryable());

            // Act
            var result = _service.UzmanlıkEkle("Nöroloji");

            // Assert
            result.IsSuccess.Should().BeTrue();
            _mockRepo.Verify(x => x.Create(It.IsAny<Uzmanlık>()), Times.Once);
        }
    }
}