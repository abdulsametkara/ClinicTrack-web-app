# ClinickTrack

ClinickTrack, modern bir klinik randevu takip ve yönetim sistemidir. Doktorlar, hastalar ve yöneticiler için kapsamlı bir platform sunar.

## 📋 İçindekiler

- [Özellikler](#özellikler)
- [Teknolojiler](#teknolojiler)
- [Proje Yapısı](#proje-yapısı)
- [Kurulum](#kurulum)
- [Kullanım](#kullanım)
- [API Dokümantasyonu](#api-dokümantasyonu)
- [Test](#test)
- [Docker](#docker)
- [Katkıda Bulunma](#katkıda-bulunma)

## ✨ Özellikler

### Kullanıcı Yönetimi

- Kullanıcı kayıt ve giriş sistemi
- JWT tabanlı kimlik doğrulama
- Rol tabanlı yetkilendirme (Admin, Doktor, Hasta)
- İlk girişte parola belirleme
- Profil yönetimi (email, telefon, parola güncelleme)

### Randevu Yönetimi

- Randevu oluşturma, güncelleme ve silme
- Randevu sorgulama ve filtreleme
- Doktor ve hasta bazlı randevu listeleme
- Randevu durumu takibi

### Doktor Yönetimi

- Doktor kayıt ve bilgi yönetimi
- Uzmanlık alanı tanımlama
- Doktor listeleme ve arama

### Hasta Yönetimi

- Hasta kayıt sistemi
- Hasta bilgileri yönetimi
- Hasta listeleme ve sorgulama

### Dashboard

- İstatistiksel veriler
- Kullanıcı bazlı özel dashboard'lar
- Admin, Doktor ve Hasta için özelleştirilmiş görünümler

## 🛠️ Teknolojiler

### Backend

- **.NET 8.0** - Web API framework
- **Entity Framework Core 9.0** - ORM
- **SQL Server** - Veritabanı
- **JWT Bearer Authentication** - Kimlik doğrulama
- **Swagger/OpenAPI** - API dokümantasyonu
- **xUnit** - Unit test framework

### Frontend

- **React 19** - UI framework
- **Vite** - Build tool
- **React Router DOM** - Routing
- **Tailwind CSS** - Styling
- **Axios** - HTTP client
- **SweetAlert2** - Alert/Modal componentleri
- **Lucide React** - Icon library

### Mimari

- **Clean Architecture** - Katmanlı mimari yapısı
- **Repository Pattern** - Veri erişim deseni
- **Dependency Injection** - Bağımlılık yönetimi
- **DTO Pattern** - Veri transfer nesneleri

## 📁 Proje Yapısı

```
ClinickTrack/
├── backend/
│   ├── ClinickCore/              # Domain katmanı (Entities, DTOs)
│   │   ├── Entities/             # Veritabanı entity'leri
│   │   └── DTOs/                 # Veri transfer nesneleri
│   │
│   ├── ClinickDataAccess/        # Veri erişim katmanı
│   │   ├── Repository/           # Repository implementasyonları
│   │   ├── Migrations/           # Entity Framework migrations
│   │   └── DatabaseBaglanti.cs   # DbContext
│   │
│   ├── ClinickService/           # İş mantığı katmanı
│   │   ├── Interfaces/           # Service interface'leri
│   │   └── Services/             # Service implementasyonları
│   │
│   └── ClinickTrack/             # API katmanı
│       ├── Controllers/          # API controller'ları
│       ├── Program.cs            # Uygulama başlangıç noktası
│       ├── appsettings.json     # Yapılandırma dosyası
│       └── ClinickTrack.UnitTests/  # Unit testler
│
└── frontend/
    ├── src/
    │   ├── pages/                # Sayfa componentleri
    │   ├── api.js                # API client
    │   └── App.jsx               # Ana uygulama componenti
    ├── package.json
    └── vite.config.js
```

## 🚀 Kurulum

### Gereksinimler

- .NET 8.0 SDK
- SQL Server (veya SQL Server Express)
- Node.js 18+ ve npm
- Git

### Backend Kurulumu

1. Projeyi klonlayın:
   
   ```bash
   git clone <repository-url>
   cd ClinickTrack/backend
   ```

2. Veritabanı bağlantı string'ini `ClinickTrack/appsettings.json` dosyasında yapılandırın:
   
   ```json
   {
   "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=Clinick;User Id=sa;Password=YourPassword;TrustServerCertificate=True;"
   }
   }
   ```

3. JWT ayarlarını yapılandırın (appsettings.json):
   
   ```json
   {
   "JwtSettings": {
    "Key": "YourSecretKeyHere",
    "Issuer": "ClinickTrack",
    "Audience": "ClinickTrackUsers",
    "ExpiryInMinutes": 20
   }
   }
   ```

4. Entity Framework migrations'ları çalıştırın:
   
   ```bash
   cd ClinickTrack
   dotnet ef database update --project ../ClinickDataAccess
   ```

5. Uygulamayı çalıştırın:
   
   ```bash
   dotnet run
   ```

API varsayılan olarak `https://localhost:5001` veya `http://localhost:5000` adresinde çalışacaktır.

### Frontend Kurulumu

1. Frontend dizinine gidin:
   
   ```bash
   cd ../frontend
   ```

2. Bağımlılıkları yükleyin:
   
   ```bash
   npm install
   ```

3. Geliştirme sunucusunu başlatın:
   
   ```bash
   npm run dev
   ```

Frontend varsayılan olarak `http://localhost:5173` adresinde çalışacaktır.

## 📖 Kullanım

### API Endpoint'leri

Temel endpoint'ler:

- **Kullanıcı İşlemleri**: `/api/Kullanici`
  
  - `POST /giris` - Kullanıcı girişi
  - `POST /kayitOl` - Hasta kaydı
  - `POST /ilkParolaBelirle` - İlk parola belirleme
  - `GET /profil` - Kullanıcı profili

- **Randevu İşlemleri**: `/api/Randevu`
  
  - `GET /getAll` - Tüm randevular
  - `POST /create` - Randevu oluşturma
  - `PUT /update/{id}` - Randevu güncelleme
  - `DELETE /delete/{id}` - Randevu silme

- **Doktor İşlemleri**: `/api/Doktor`
  
  - `GET /getAll` - Tüm doktorlar
  - `POST /create` - Doktor oluşturma
  - `GET /getById/{id}` - Doktor detayı

- **Hasta İşlemleri**: `/api/Hasta`
  
  - `GET /getAll` - Tüm hastalar
  - `GET /getById/{id}` - Hasta detayı

- **Uzmanlık İşlemleri**: `/api/Uzmanlik`
  
  - `GET /getAll` - Tüm uzmanlık alanları

### Kimlik Doğrulama

API'yi kullanmak için JWT token gereklidir. Token'ı almak için:

```bash
POST /api/Kullanici/giris
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "password123"
}
```

Yanıt olarak dönen token'ı, sonraki isteklerde `Authorization` header'ında kullanın:

```
Authorization: Bearer <your-token-here>
```

## 🧪 Test

### Unit Testleri Çalıştırma

```bash
cd ClinickTrack/ClinickTrack.UnitTests
dotnet test
```

### Coverage Raporu

Coverage raporu oluşturmak için:

```bash
# Windows
02run-coverage.bat

# Manuel
dotnet test --collect:"XPlat Code Coverage" --settings:coverage.runsettings
```

Coverage raporları `coverage/` dizininde HTML formatında oluşturulur.

## 🐳 Docker

### Docker ile Çalıştırma

1. Dockerfile'ı kullanarak image oluşturun:
   
   ```bash
   docker build -t clinicktrack-api .
   ```

2. Container'ı çalıştırın:
   
   ```bash
   docker run -p 5000:80 clinicktrack-api
   ```

**Not**: Dockerfile'daki klasör isimlerini projenize göre güncellemeyi unutmayın.

## 🔐 Güvenlik

- JWT token'lar 20 dakika geçerlidir
- Parolalar hash'lenerek saklanır
- CORS yapılandırması mevcuttur
- Rol tabanlı yetkilendirme uygulanmıştır

# 
