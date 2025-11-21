# 🧪 ClinickTrack Test Yazma Kılavuzu

## 📋 İçindekiler
1. [Test Yazmaya Başlamadan Önce](#test-yazmaya-başlamadan-önce)
2. [İlk Testinizi Yazın](#ilk-testinizi-yazın)
3. [Coverage Hedeflerimiz](#coverage-hedeflerimiz)
4. [Test Örnekleri](#test-örnekleri)
5. [Sık Karşılaşılan Durumlar](#sık-karşılaşılan-durumlar)

---

## 🎯 Test Yazmaya Başlamadan Önce

### Hangi Service'i Test Edeceğiz?

Şu an test edilmesi gereken servisler:

| Servis | Mevcut Coverage | Öncelik | Test Sayısı (Tahmini) |
|--------|----------------|---------|----------------------|
| **DoktorService** | 0% | 🔴 Yüksek | ~15 test |
| **HastaService** | 0% | 🔴 Yüksek | ~15 test |
| **UzmanlıkService** | 0% | 🟡 Orta | ~8 test |
| **RandevuService** | 10% | 🟡 Orta | +10 test |
| **KullanıcıService** | 30% | 🟢 Düşük | +5 test |

---

## 📝 İlk Testinizi Yazın

### Adım 1: Test Dosyası Oluştur

`ClinickTrack.UnitTests/Services/DoktorServiceTests.cs` dosyasını oluşturun:

```csharp
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
    public class DoktorServiceTests
    {
        private readonly Mock<IGenericRepository<Doktor>> _mockDoktorRepo;
        private readonly Mock<IGenericRepository<Kullanıcı>> _mockKullanıcıRepo;
        private readonly Mock<IGenericRepository<Uzmanlık>> _mockUzmanlıkRepo;
        private readonly DoktorService _service;

        public DoktorServiceTests()
        {
            // Mock repository'leri oluştur
            _mockDoktorRepo = new Mock<IGenericRepository<Doktor>>();
            _mockKullanıcıRepo = new Mock<IGenericRepository<Kullanıcı>>();
            _mockUzmanlıkRepo = new Mock<IGenericRepository<Uzmanlık>>();
            
            // Servis instance'ı oluştur
            _service = new DoktorService(
                _mockDoktorRepo.Object,
                _mockKullanıcıRepo.Object,
                _mockUzmanlıkRepo.Object
            );
        }

        // Testleriniz buraya gelecek...
    }
}
```

### Adım 2: İlk Test - Valid Data

```csharp
[Fact]
public void DoktorEkle_ValidData_ReturnsSuccess()
{
    // Arrange (Hazırlık)
    var dto = new DoktorOlusturDto
    {
        İsim = "Ahmet",
        Soyisim = "Yılmaz",
        TCNo = "12345678901",
        Email = "ahmet.yilmaz@clinick.com",
        TelefonNumarası = "05551234567",
        UzmanlıkId = 1,
        DiplomaNo = "DIP123456"
    };

    // Boş liste döndür (email çakışması yok)
    _mockKullanıcıRepo.Setup(x => x.GetAll())
        .Returns(new List<Kullanıcı>().AsQueryable());
    
    // Uzmanlık var
    _mockUzmanlıkRepo.Setup(x => x.GetById(1))
        .Returns(new Uzmanlık { Id = 1, Ad = "Kardiyoloji" });

    // Act (İşlem)
    var result = _service.DoktorEkle(dto);

    // Assert (Doğrulama)
    result.IsSuccess.Should().BeTrue();
    result.Message.Should().Contain("başarıyla");
    
    // Repository'nin Create metodunun çağrıldığını doğrula
    _mockKullanıcıRepo.Verify(x => x.Create(It.IsAny<Kullanıcı>()), Times.Once);
    _mockDoktorRepo.Verify(x => x.Create(It.IsAny<Doktor>()), Times.Once);
}
```

### Adım 3: İkinci Test - Invalid Data

```csharp
[Fact]
public void DoktorEkle_DuplicateEmail_ReturnsFailure()
{
    // Arrange
    var dto = new DoktorOlusturDto
    {
        İsim = "Ahmet",
        Soyisim = "Yılmaz",
        Email = "existing@clinick.com",
        TCNo = "12345678901",
        TelefonNumarası = "05551234567",
        UzmanlıkId = 1
    };

    // Email zaten var
    var existingKullanıcı = new Kullanıcı
    {
        Id = 1,
        Email = "existing@clinick.com",
        İsim = "Mevcut",
        Soyisim = "Kullanıcı"
    };

    _mockKullanıcıRepo.Setup(x => x.GetAll())
        .Returns(new List<Kullanıcı> { existingKullanıcı }.AsQueryable());

    // Act
    var result = _service.DoktorEkle(dto);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Message.Should().Contain("email");
    
    // Create çağrılmamalı
    _mockKullanıcıRepo.Verify(x => x.Create(It.IsAny<Kullanıcı>()), Times.Never);
}
```

### Adım 4: Testi Çalıştır

```bash
# Sadece bu test dosyasını çalıştır
dotnet test --filter "ClassName=DoktorServiceTests"

# Verbose output ile
dotnet test --filter "ClassName=DoktorServiceTests" --logger "console;verbosity=detailed"
```

---

## 🎯 Coverage Hedeflerimiz

### Mevcut Durum: 4.56% 📉
### Hedef: 90% 📈

Her servis için yazılması gereken testler:

### 1️⃣ DoktorService (15 test)

- [x] **DoktorEkle** (5 test)
  - Valid data → Success
  - Duplicate email → Failure
  - Duplicate telefon → Failure
  - Invalid uzmanlık → Failure
  - Null data → Failure

- [ ] **DoktorGuncelle** (4 test)
  - Valid update → Success
  - Doktor not found → Failure
  - Duplicate email → Failure
  - Update same data → Success

- [ ] **DoktorSil** (2 test)
  - Doktor exists → Success
  - Doktor not found → Failure

- [ ] **DoktorGetir** (2 test)
  - By Id → Success
  - By KullanıcıId → Success

- [ ] **TümDoktorlariGetir** (1 test)
  - Returns list

- [ ] **DoktorRandevulariniGetir** (1 test)
  - Returns randevu list

### 2️⃣ HastaService (15 test)

- [ ] **HastaEkle** (5 test)
  - Valid data → Success
  - Duplicate TC → Failure
  - Duplicate email → Failure
  - Invalid data → Failure
  - Null data → Failure

- [ ] **HastaGuncelle** (4 test)
  - Valid update → Success
  - Hasta not found → Failure
  - Duplicate email → Failure
  - Partial update → Success

- [ ] **HastaSil** (2 test)
  - Hasta exists → Success
  - Hasta not found → Failure

- [ ] **HastaGetir** (2 test)
  - By Id → Success
  - By KullanıcıId → Success

- [ ] **TümHastalariGetir** (1 test)
  - Returns list

- [ ] **HastaRandevulariniGetir** (1 test)
  - Returns randevu list

### 3️⃣ RandevuService (+10 test)

Mevcut: RandevuUygunMu testleri var

- [ ] **RandevuEkle** (5 test)
  - Valid randevu → Success
  - Past date → Failure
  - Conflict → Failure
  - Invalid doktor → Failure
  - Invalid hasta → Failure

- [ ] **RandevuGuncelle** (3 test)
  - Update status → Success
  - Add notes → Success
  - Randevu not found → Failure

- [ ] **RandevuSil** (1 test)
  - Randevu exists → Success

- [ ] **RandevuGetir** (1 test)
  - By Id → Success

### 4️⃣ UzmanlıkService (8 test)

- [ ] **UzmanlıkEkle** (3 test)
  - Valid data → Success
  - Duplicate name → Failure
  - Empty name → Failure

- [ ] **UzmanlıkGuncelle** (2 test)
  - Valid update → Success
  - Not found → Failure

- [ ] **UzmanlıkSil** (1 test)
  - Uzmanlık exists → Success

- [ ] **UzmanlıkGetir** (1 test)
  - By Id → Success

- [ ] **TümUzmanlıklariGetir** (1 test)
  - Returns list

---

## 📚 Test Örnekleri

### Örnek 1: Null Check Testi

```csharp
[Fact]
public void DoktorEkle_NullDto_ReturnsFailure()
{
    // Act
    var result = _service.DoktorEkle(null);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Message.Should().NotBeNullOrEmpty();
}
```

### Örnek 2: Exception Handling Testi

```csharp
[Fact]
public void DoktorGetir_RepositoryThrowsException_ReturnsFailure()
{
    // Arrange
    _mockDoktorRepo.Setup(x => x.GetById(It.IsAny<int>()))
        .Throws(new Exception("Database error"));

    // Act
    var result = _service.DoktorGetir(1);

    // Assert
    result.IsSuccess.Should().BeFalse();
    result.Message.Should().Contain("hata");
}
```

### Örnek 3: List Test

```csharp
[Fact]
public void TümDoktorlariGetir_ReturnsAllDoctors()
{
    // Arrange
    var doktorlar = new List<Doktor>
    {
        new Doktor { Id = 1, KullanıcıId = 1 },
        new Doktor { Id = 2, KullanıcıId = 2 },
        new Doktor { Id = 3, KullanıcıId = 3 }
    }.AsQueryable();

    _mockDoktorRepo.Setup(x => x.GetAll())
        .Returns(doktorlar);

    // Act
    var result = _service.TümDoktorlariGetir();

    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Data.Should().HaveCount(3);
}
```

### Örnek 4: Theory ile Parametreli Test

```csharp
[Theory]
[InlineData("")]
[InlineData(" ")]
[InlineData(null)]
public void DoktorEkle_InvalidEmail_ReturnsFailure(string email)
{
    // Arrange
    var dto = new DoktorOlusturDto
    {
        Email = email,
        İsim = "Test",
        Soyisim = "Doktor"
    };

    // Act
    var result = _service.DoktorEkle(dto);

    // Assert
    result.IsSuccess.Should().BeFalse();
}
```

---

## 🔍 Sık Karşılaşılan Durumlar

### 1. IQueryable Mock'lama

```csharp
// ✅ Doğru
var mockData = new List<Entity>().AsQueryable();
_mockRepo.Setup(x => x.GetAll()).Returns(mockData);

// ❌ Yanlış
_mockRepo.Setup(x => x.GetAll()).Returns(new List<Entity>());
```

### 2. Async Method Mock'lama

```csharp
// Eğer servis async metodlar içeriyorsa:
_mockRepo.Setup(x => x.GetByIdAsync(It.IsAny<int>()))
    .ReturnsAsync(new Entity());
```

### 3. Callback ile ID Atama

```csharp
_mockRepo.Setup(x => x.Create(It.IsAny<Entity>()))
    .Callback<Entity>(e => e.Id = 1);
```

### 4. Verify ile Method Çağrısı Kontrolü

```csharp
// Bir kez çağrılmalı
_mockRepo.Verify(x => x.Create(It.IsAny<Entity>()), Times.Once);

// Hiç çağrılmamalı
_mockRepo.Verify(x => x.Delete(It.IsAny<Entity>()), Times.Never);

// En az bir kez
_mockRepo.Verify(x => x.GetAll(), Times.AtLeastOnce);
```

---

## 🚀 Test Yazma İş Akışı

### 1. Service Metodunu Belirle
```
Örnek: DoktorService.DoktorEkle()
```

### 2. Senaryoları Listele
- ✅ Valid data → Success
- ❌ Duplicate email → Failure
- ❌ Invalid TC → Failure
- ❌ Null data → Failure

### 3. Her Senaryo için Test Yaz
```csharp
[Fact] public void DoktorEkle_ValidData_ReturnsSuccess() { ... }
[Fact] public void DoktorEkle_DuplicateEmail_ReturnsFailure() { ... }
[Fact] public void DoktorEkle_InvalidTC_ReturnsFailure() { ... }
```

### 4. Testleri Çalıştır
```bash
dotnet test
```

### 5. Coverage Kontrol Et
```bash
.\run-coverage.bat
start coverage\index.html
```

### 6. Coverage %90'ın Altındaysa → Daha Fazla Test Ekle

---

## 📊 Coverage Kontrolü

### Hangi Satırlar Test Edilmedi?

`coverage/index.html` dosyasını açın:

- 🟢 **Yeşil satırlar:** Test edilmiş ✅
- 🔴 **Kırmızı satırlar:** Test edilmemiş ❌
- 🟡 **Sarı satırlar:** Kısmen test edilmiş ⚠️

### Coverage Artırma Stratejisi

1. **Kırmızı satırları bulun** (test edilmemiş)
2. **Bu satırları tetikleyen senaryolar yazın**
3. **Yeniden coverage çalıştırın**
4. **%90'a ulaşana kadar tekrarlayın**

---

## ✅ Checklist

Test yazarken kontrol et:

- [ ] Her metod için en az 1 success testi var mı?
- [ ] Her metod için en az 1 failure testi var mı?
- [ ] Null/empty input testleri var mı?
- [ ] Exception handling test edilmiş mi?
- [ ] Repository method'ları Verify edilmiş mi?
- [ ] Test isimleri açıklayıcı mı? (MethodName_Scenario_Expected)
- [ ] AAA pattern kullanılmış mı? (Arrange, Act, Assert)
- [ ] FluentAssertions kullanılmış mı?

---

## 🎓 İyi Uygulama Örnekleri

### ✅ İyi Test
```csharp
[Fact]
public void DoktorEkle_ValidDataWithAllFields_CreatesDoktorSuccessfully()
{
    // Arrange
    var dto = CreateValidDoktorDto(); // Helper method
    SetupMocksForSuccess(); // Helper method
    
    // Act
    var result = _service.DoktorEkle(dto);
    
    // Assert
    result.IsSuccess.Should().BeTrue();
    result.Message.Should().Contain("başarıyla");
    VerifyRepositoryCalls(); // Helper method
}
```

### ❌ Kötü Test
```csharp
[Fact]
public void Test1()
{
    var x = new DoktorOlusturDto();
    var y = _service.DoktorEkle(x);
    Assert.True(y.IsSuccess);
}
```

---

**Başarılar! Test yazmaya başlayın ve coverage'ı %90'a çıkaralım! 🚀**







