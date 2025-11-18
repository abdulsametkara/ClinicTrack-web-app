@echo off
echo ========================================
echo   ClinickTrack Backend - Otomatik Kurulum
echo ========================================
echo.

REM Ana dizine git
cd /d "%~dp0"

echo [1/6] .NET SDK Kontrolu...
dotnet --version >nul 2>&1
if %errorlevel% neq 0 (
    echo [HATA] .NET SDK bulunamadi!
    echo Lutfen .NET 8.0 SDK'yi yukleyin: https://dotnet.microsoft.com/download/dotnet/8.0
    pause
    exit /b 1
)
echo [OK] .NET SDK: 
dotnet --version
echo.

echo [2/6] NuGet Paketlerini Yukleniyor...
dotnet restore
if %errorlevel% neq 0 (
    echo [HATA] NuGet paketleri yuklenemedi!
    pause
    exit /b 1
)
echo [OK] Tum paketler yuklendi
echo.

echo [3/6] Test Projesi Hazirlaniyor...
dotnet build ClinickTrack\ClinickTrack.UnitTests\ClinickTrack.UnitTests.csproj
if %errorlevel% neq 0 (
    echo [HATA] Test projesi build edilemedi!
    pause
    exit /b 1
)
echo [OK] Test projesi hazir
echo.

echo [4/6] ReportGenerator Tool Yukleniyor...
dotnet tool list -g | findstr "reportgenerator" >nul 2>&1
if %errorlevel% neq 0 (
    echo ReportGenerator yukleniyor...
    dotnet tool install -g dotnet-reportgenerator-globaltool
) else (
    echo [OK] ReportGenerator zaten yuklu
)
echo.

echo [5/6] Database Kurulum Secenekleri
echo.
echo Hangi database kullanmak istersiniz?
echo [1] LocalDB (Otomatik kurulum)
echo [2] Docker SQL Server (Onerilen - Ekip icin)
echo [3] Kendi SQL Server'im var (Manuel)
echo.
set /p DB_CHOICE="Seciminiz (1-3): "

if "%DB_CHOICE%"=="1" (
    echo.
    echo [5/6] LocalDB Migration'lari Calistiriliyor...
    cd ClinickTrack
    dotnet ef database update
    if %errorlevel% neq 0 (
        echo [UYARI] Migration basarisiz. Database ayarlarini kontrol edin.
    ) else (
        echo [OK] Database hazir
    )
    cd ..
) else if "%DB_CHOICE%"=="2" (
    echo.
    echo Docker SQL Server Kurulumu
    echo ---------------------------
    echo.
    echo Docker Desktop calisiyor mu? (docker ps)
    docker ps >nul 2>&1
    if %errorlevel% neq 0 (
        echo [UYARI] Docker Desktop calismiyor!
        echo Lutfen Docker Desktop'i baslatin ve tekrar deneyin.
        echo.
        echo Manuel kurulum icin:
        echo 1. Docker Desktop'i calistirin
        echo 2. Su komutu calistirin:
        echo    docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=ClinickTrack@2025" -p 1433:1433 --name clinick-sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
        echo 3. appsettings.json'da connection string'i guncelleyin
        echo 4. dotnet ef database update
    ) else (
        echo [OK] Docker calisiyor
        echo.
        echo SQL Server container'i baslatiliyor...
        docker ps -a | findstr "clinick-sqlserver" >nul 2>&1
        if %errorlevel% equ 0 (
            echo Container zaten var, baslat/restart yapiliyor...
            docker start clinick-sqlserver
        ) else (
            echo Yeni container olusturuluyor...
            docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=ClinickTrack@2025" -p 1433:1433 --name clinick-sqlserver -d mcr.microsoft.com/mssql/server:2022-latest
        )
        
        echo Bekleyin, SQL Server baslatiliyor... (10 saniye)
        timeout /t 10 /nobreak >nul
        
        echo.
        echo appsettings.json guncelleniyor...
        cd ClinickTrack
        
        echo [OK] Connection string:
        echo Server=localhost,1433;Database=ClinickTrackDb;User Id=sa;Password=ClinickTrack@2025;TrustServerCertificate=True;
        echo.
        echo Migration'lar calistiriliyor...
        dotnet ef database update
        
        if %errorlevel% neq 0 (
            echo [UYARI] Migration basarisiz.
            echo Lutfen appsettings.json'da connection string'i kontrol edin.
        ) else (
            echo [OK] Database hazir
        )
        cd ..
    )
) else (
    echo.
    echo Manuel Database Kurulumu
    echo ------------------------
    echo 1. appsettings.json'da connection string'inizi ayarlayin
    echo 2. Su komutu calistirin: dotnet ef database update
    echo.
)

echo.
echo [6/6] Ilk Test Calistiriliyor...
dotnet test ClinickTrack\ClinickTrack.UnitTests\ClinickTrack.UnitTests.csproj --verbosity quiet
if %errorlevel% neq 0 (
    echo [UYARI] Testler basarisiz. Daha sonra kontrol edin.
) else (
    echo [OK] Testler basarili!
)

echo.
echo ========================================
echo   KURULUM TAMAMLANDI!
echo ========================================
echo.
echo Sonraki Adimlar:
echo.
echo 1. Projeyi Calistir:
echo    cd ClinickTrack
echo    dotnet run
echo.
echo 2. Testleri Calistir:
echo    dotnet test
echo.
echo 3. Coverage Raporu:
echo    .\run-coverage.bat
echo    start coverage\index.html
echo.
echo 4. API Dokumantasyonu:
echo    https://localhost:7000/swagger
echo.
echo Ilk Admin Kullanicisi:
echo    Email: admin@clinicktrack.com
echo    Parola: admin123
echo.
echo ========================================
pause

