@echo off
cd /d "%~dp0"
echo ========================================
echo   ClinickTrack Test Coverage Report
echo ========================================
echo.

REM Önceki coverage klasörünü tamamen temizle
if exist "coverage\" rd /s /q "coverage"
if exist "coverage-temp\" rd /s /q "coverage-temp"
if exist "TestResults\" rd /s /q "TestResults"

echo [1/2] Running Unit Tests with coverage...
dotnet test ClinickTrack\ClinickTrack.UnitTests\ClinickTrack.UnitTests.csproj ^
    /p:CollectCoverage=true ^
    /p:CoverletOutputFormat=cobertura ^
    /p:CoverletOutput=..\..\coverage-temp\ ^
    /p:Exclude="[xunit.*]*"

if %errorlevel% neq 0 (
    echo [ERROR] Unit tests failed!
    exit /b 1
)

echo.
echo [2/2] Generating HTML coverage report...

REM Coverage dosyası
set COVERAGE_FILES=coverage-temp\coverage.cobertura.xml

reportgenerator ^
    -reports:"%COVERAGE_FILES%" ^
    -targetdir:"coverage" ^
    -reporttypes:"Html;Badges"

if %errorlevel% neq 0 (
    echo [ERROR] Report generation failed!
    exit /b 1
)

REM Geçici dosyaları temizle
if exist "coverage-temp\" rd /s /q "coverage-temp"
if exist "TestResults\" rd /s /q "TestResults"

echo.
echo ========================================
echo   COVERAGE REPORT READY!
echo   Location: coverage\index.html
echo ========================================
echo.

pause