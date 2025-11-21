# 1. Build Aşaması
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Proje dosyalarını kopyala (Buradaki klasör isimlerini kendi projene göre düzenle)
COPY ["ClinickAPI/ClinickAPI.csproj", "ClinickAPI/"]
COPY ["ClinickCore/ClinickCore.csproj", "ClinickCore/"]
COPY ["ClinickDataAccess/ClinickDataAccess.csproj", "ClinickDataAccess/"]
COPY ["ClinickService/ClinickService.csproj", "ClinickService/"]

# Bağımlılıkları yükle
RUN dotnet restore "ClinickAPI/ClinickAPI.csproj"

# Tüm dosyaları kopyala ve build al
COPY . .
WORKDIR "/src/ClinickAPI"
RUN dotnet build "ClinickAPI.csproj" -c Release -o /app/build

# Publish al
FROM build AS publish
RUN dotnet publish "ClinickAPI.csproj" -c Release -o /app/publish

# 2. Run Aşaması
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ClinickAPI.dll"]