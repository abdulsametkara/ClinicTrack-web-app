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
    public class HastaServiceTests
    {
        private readonly Mock<IGenericRepository<Hasta>> _mockRepo;
        private readonly HastaService _service;

        public HastaServiceTests()
        {
            _mockRepo = new Mock<IGenericRepository<Hasta>>();
            _service = new HastaService(_mockRepo.Object);
        }

        [Fact]
        public void HastaEkle_KullaniciZatenHastaysa_HataDonmeli()
        {
            // Arrange
            var dto = new HastaOlusturDto { KullanıcıId = 5 };
            _mockRepo.Setup(x => x.GetAll()).Returns(new List<Hasta>
            {
                new Hasta { KullanıcıId = 5 } // Zaten var
            }.AsQueryable());

            // Act
            var result = _service.HastaEkle(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("zaten mevcut");
        }

        [Fact]
        public void HastaEkle_Basarili_HastaOlusturmalı()
        {
            // Arrange
            var dto = new HastaOlusturDto { KullanıcıId = 10, Cinsiyet = "Erkek" };
            _mockRepo.Setup(x => x.GetAll()).Returns(new List<Hasta>().AsQueryable());

            // Act
            var result = _service.HastaEkle(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
            _mockRepo.Verify(x => x.Create(It.IsAny<Hasta>()), Times.Once);
        }
    }
}