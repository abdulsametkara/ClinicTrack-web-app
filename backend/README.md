# 🏥 ClinickTrack Backend - Kurulum ve Test Yazma Rehberi

## 📋 İçindekiler
1. [Gereksinimler](#gereksinimler)
2. [Hızlı Başlangıç](#hızlı-başlangıç)
3. [Manuel Kurulum](#manuel-kurulum)
4. [Test Yazma](#test-yazma)
5. [Coverage Raporu](#coverage-raporu)

---

## 🔧 Gereksinimler

Projeyi çalıştırmak için sisteminizde aşağıdakilerin yüklü olması gerekir:

- **[.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)** veya üzeri
- **SQL Server** (LocalDB, Docker, veya SQL Server Express)
- **Git** (proje klonlamak için)
- **Docker Desktop** (Opsiyonel - Ortak SQL Server için önerilir)

---

## 🚀 Hızlı Başlangıç (Otomatik Kurulum)

### 1. Projeyi İndirin
```bash
git clone <repo-url>
cd ClinickTrack/backend
```

### 2. Otomatik Kurulum Scriptini Çalıştırın
```bash
setup.bat
```

Bu script otomatik olarak:
✅ .NET SDK kontrolü yapar
✅ Gerekli NuGet paketlerini yükler
✅ ReportGenerator tool'unu yükler
✅ Database migration'larını çalıştırır
✅ Test projesini hazırlar

### 3. Projeyi Çalıştırın
```bash
cd ClinickTrack
dotnet run
```

API: `https://localhost:7000` veya `http://localhost:5000`

---

## 📝 Manuel Kurulum

### Adım 1: NuGet Paketlerini Yükle
```bash
dotnet restore
```

### Adım 2: Database Migration'larını Çalıştır

**LocalDB kullanıyorsanız:**
```bash
cd ClinickTrack
dotnet ef database update
```

**Docker SQL Server kullanıyorsanız:**
```bash
# SQL Server Container'ı başlat
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=ClinickTrack@2025" -p 1433:1433 --name clinick-sqlserver -d mcr.microsoft.com/mssql/server:2022-latest

# Connection string'i appsettings.json'da güncelle:
"Server=localhost,1433;Database=ClinickTrackDb;User Id=sa;Password=ClinickTrack@2025;TrustServerCertificate=True;"

# Migration'ları çalıştır
cd ClinickTrack
dotnet ef database update
```

### Adım 3: İlk Admin Kullanıcısı

Sistem otomatik olarak seed data ile ilk admin kullanıcısını oluşturur:

```
Email: admin@clinicktrack.com
Parola: admin123
```

---

## 🧪 Test Yazma

### Test Projesi Yapısı

```
ClinickTrack.UnitTests/
├── Services/
│   ├── KullanıcıServiceTests.cs    ✅ (Örnek testler mevcut)
│   ├── RandevuServiceTests.cs      ✅ (Örnek testler mevcut)
│   ├── DoktorServiceTests.cs       ⚠️ (Yazılacak)
│   ├── HastaServiceTests.cs        ⚠️ (Yazılacak)
│   └── UzmanlıkServiceTests.cs     ⚠️ (Yazılacak)
```

### Test Yazma Şablonu

```csharp
using Xunit;
using Moq;
using FluentAssertions;
using ClinickService.Services;
using ClinickDataAccess.Repository;
using ClinickCore.Entities;
using ClinickCore.DTOs;

namespace ClinickTrack.Tests.Services
{
    public class DoktorServiceTests
    {
        private readonly Mock<IGenericRepository<Doktor>> _mockDoktorRepo;
        private readonly Mock<IGenericRepository<Kullanıcı>> _mockKullanıcıRepo;
        private readonly DoktorService _service;

        public DoktorServiceTests()
        {
            _mockDoktorRepo = new Mock<IGenericRepository<Doktor>>();
            _mockKullanıcıRepo = new Mock<IGenericRepository<Kullanıcı>>();
            _service = new DoktorService(_mockDoktorRepo.Object, _mockKullanıcıRepo.Object);
        }

        [Fact]
        public void DoktorEkle_ValidData_ReturnsSuccess()
        {
            // Arrange
            var dto = new DoktorOlusturDto
            {
                İsim = "Dr. Test",
                Soyisim = "Doktor",
                Email = "test@doctor.com",
                // ... diğer alanlar
            };

            _mockKullanıcıRepo.Setup(x => x.GetAll())
                .Returns(new List<Kullanıcı>().AsQueryable());

            // Act
            var result = _service.DoktorEkle(dto);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public void DoktorEkle_DuplicateEmail_ReturnsFailure()
        {
            // Arrange
            var dto = new DoktorOlusturDto
            {
                Email = "existing@doctor.com",
                // ...
            };

            _mockKullanıcıRepo.Setup(x => x.GetAll())
                .Returns(new List<Kullanıcı>
                {
                    new Kullanıcı { Email = "existing@doctor.com" }
                }.AsQueryable());

            // Act
            var result = _service.DoktorEkle(dto);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Message.Should().Contain("email");
        }
    }
}
```

### Test Çalıştırma

**Tüm testleri çalıştır:**
```bash
dotnet test
```

**Sadece bir test dosyasını çalıştır:**
```bash
dotnet test --filter "ClassName=KullanıcıServiceTests"
```

**Verbose output ile:**
```bash
dotnet test --logger "console;verbosity=detailed"
```

---

## 📊 Coverage Raporu

### Coverage'ı Çalıştır

```bash
# Backend klasöründen
.\run-coverage.bat
```

Bu script:
1. ✅ Tüm testleri çalıştırır
2. ✅ Coverage verilerini toplar
3. ✅ HTML raporu oluşturur (`coverage/index.html`)

### Coverage Raporunu Görüntüle

Script bittiğinde:
```bash
# Windows
start coverage\index.html

# Manuel
coverage\index.html  # Dosyayı tarayıcıda aç
```

### Coverage Hedefleri

| Modül | Mevcut | Hedef |
|-------|--------|-------|
| **ClinickCore** | 27% | 80% |
| **ClinickService** | 10% | 90% |
| **ClinickDataAccess** | 0% | 40% |
| **TOPLAM** | **4.56%** | **90%** |

---

## 🎯 Test Yazma İpuçları

### 1. AAA Pattern Kullanın
```csharp
[Fact]
public void MethodName_Scenario_ExpectedResult()
{
    // Arrange (Hazırlık)
    var input = new TestData();
    
    // Act (İşlem)
    var result = _service.Method(input);
    
    // Assert (Doğrulama)
    result.Should().BeTrue();
}
```

### 2. Her Senaryo İçin Ayrı Test
- ✅ Valid data → Success
- ✅ Invalid data → Failure
- ✅ Null data → Exception
- ✅ Duplicate data → Failure
- ✅ Empty list → Empty result

### 3. Mock Kullanımı
```csharp
// Repository'yi mock'la
_mockRepo.Setup(x => x.GetAll())
    .Returns(testData.AsQueryable());

// Method çağrıldığını doğrula
_mockRepo.Verify(x => x.Create(It.IsAny<Entity>()), Times.Once);
```

### 4. FluentAssertions Kullanın
```csharp
// Daha okunabilir assertions
result.Should().BeTrue();
result.Should().NotBeNull();
result.Message.Should().Contain("error");
list.Should().HaveCount(5);
```

---

## 📦 Kullanılan Paketler

### API Projeleri
- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT token doğrulama
- `Microsoft.EntityFrameworkCore` - ORM
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI

### Test Projeleri
- `xUnit` - Test framework
- `Moq` - Mocking library
- `FluentAssertions` - Assertion library
- `coverlet.collector` - Coverage toplama
- `coverlet.msbuild` - MSBuild entegrasyonu

### Global Tools
- `dotnet-reportgenerator-globaltool` - HTML coverage raporu

---

## 🐛 Sorun Giderme

### "dotnet command not found"
.NET SDK'yı yükleyin: https://dotnet.microsoft.com/download

### "SQL Server connection failed"
1. SQL Server'ın çalıştığından emin olun
2. Connection string'i kontrol edin (`appsettings.json`)
3. Docker kullanıyorsanız: `docker ps` ile container'ın çalıştığını doğrulayın

### "Migration failed"
```bash
# Migration'ları sıfırla
dotnet ef database drop --force
dotnet ef database update
```

### "Test discovery failed"
```bash
# Test projesini temizle ve yeniden build et
dotnet clean
dotnet build
dotnet test
```

---

## 📚 Ek Kaynaklar

- [xUnit Dokümantasyonu](https://xunit.net/)
- [Moq Quickstart](https://github.com/moq/moq4/wiki/Quickstart)
- [FluentAssertions Dokümantasyonu](https://fluentassertions.com/introduction)
- [Coverlet Kullanımı](https://github.com/coverlet-coverage/coverlet)

---

## 👥 Katkıda Bulunma

1. Branch oluştur (`git checkout -b feature/YeniOzellik`)
2. Testleri yaz ve çalıştır
3. Coverage'ı kontrol et (`.\run-coverage.bat`)
4. Commit yap (`git commit -m 'Yeni özellik eklendi'`)
5. Push yap (`git push origin feature/YeniOzellik`)
6. Pull Request oluştur

---

## 📞 İletişim

Sorularınız için: [proje-email@example.com]

**Happy Testing! 🧪✨**




