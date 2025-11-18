using Xunit;
using Moq;
using FluentAssertions;
using ClinickService.Services;
using ClinickDataAccess.Repository;
using ClinickCore.Entities;
using ClinickCore.DTOs;
using System.Linq;

namespace ClinickTrack.Tests.Services
{
    public class RandevuServiceTests
    {
        private readonly Mock<IGenericRepository<Doktor>> _mockDoktorRepo;
        private readonly Mock<IGenericRepository<Hasta>> _mockHastaRepo;
        private readonly Mock<IGenericRepository<Uzmanlık>> _mockUzmanlıkRepo;
        private readonly Mock<IGenericRepository<Randevu>> _mockRandevuRepo;
        private readonly RandevuService _service;

        public RandevuServiceTests()
        {
            _mockDoktorRepo = new Mock<IGenericRepository<Doktor>>();
            _mockHastaRepo = new Mock<IGenericRepository<Hasta>>();
            _mockUzmanlıkRepo = new Mock<IGenericRepository<Uzmanlık>>();
            _mockRandevuRepo = new Mock<IGenericRepository<Randevu>>();
            
            _service = new RandevuService(
                _mockDoktorRepo.Object,
                _mockHastaRepo.Object,
                _mockUzmanlıkRepo.Object,
                _mockRandevuRepo.Object
            );
        }

        [Fact]
        public void RandevuUygunMu_NoConflict_ReturnsTrue()
        {
            // Arrange
            var randevuTarihi = DateTime.Now.AddDays(7);
            int doktorId = 1;

            var randevular = new List<Randevu>().AsQueryable();
            _mockRandevuRepo.Setup(x => x.GetAll()).Returns(randevular);

            // Act
            var result = _service.RandevuUygunMu(doktorId, randevuTarihi);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void RandevuUygunMu_WithConflict_ReturnsFalse()
        {
            // Arrange
            var randevuTarihi = DateTime.Now.AddDays(7);
            int doktorId = 1;

            var randevular = new List<Randevu>
            {
                new Randevu
                {
                    Id = 1,
                    DoktorId = doktorId,
                    RandevuTarihi = randevuTarihi,
                    HastaId = 1,
                    Durum = "Beklemede",
                    HastaŞikayeti = "Test",
                    DoktorNotları = "Test"
                }
            }.AsQueryable();

            _mockRandevuRepo.Setup(x => x.GetAll()).Returns(randevular);

            // Act
            var result = _service.RandevuUygunMu(doktorId, randevuTarihi);

            // Assert
            result.Should().BeFalse();
        }
    }
}

