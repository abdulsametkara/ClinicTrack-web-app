using ClinickCore.DTOs;
using ClinickCore.Entities;
using ClinickDataAccess.Repository;
using ClinickService.Interfaces;
using ClinickService.Response;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ClinickService.Services
{
    public class KullanıcıService : IKullanıcıService
    {
        private readonly IGenericRepository<Kullanıcı> _kullanıcıRepository;
        private readonly IGenericRepository<Hasta> _hastaRepository;
        private readonly IConfiguration _configuration;
        
        public KullanıcıService(
            IGenericRepository<Kullanıcı> kullanıcıRepository, 
            IGenericRepository<Hasta> hastaRepository,
            IConfiguration configuration)
        {
            _kullanıcıRepository = kullanıcıRepository;
            _hastaRepository = hastaRepository;
            _configuration = configuration;
        }

        public ResponseGeneric<Kullanıcı> KullanıcıOlustur(KullanıcıOlusturDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Parola))
                {
                    return ResponseGeneric<Kullanıcı>.Error("Email ve parola boş olamaz.");
                }

                if (string.IsNullOrEmpty(dto.İsim) || string.IsNullOrEmpty(dto.Soyisim))
                {
                    return ResponseGeneric<Kullanıcı>.Error("İsim ve soyisim boş olamaz.");
                }

                if (string.IsNullOrEmpty(dto.Rol))
                {
                    return ResponseGeneric<Kullanıcı>.Error("Rol bilgisi boş olamaz.");
                }

                var izinliRoller = new[] { "Admin", "Doktor", "Hasta" };
                if (!izinliRoller.Contains(dto.Rol))
                {
                    return ResponseGeneric<Kullanıcı>.Error("Geçersiz rol. İzin verilen roller: Admin, Doktor, Hasta");
                }

                if (string.IsNullOrEmpty(dto.TCNo))
                {
                    return ResponseGeneric<Kullanıcı>.Error("TC No boş olamaz.");
                }

                if (dto.TCNo.Length != 11 || !dto.TCNo.All(char.IsDigit))
                {
                    return ResponseGeneric<Kullanıcı>.Error("TC No 11 haneli sayısal değer olmalıdır.");
                }

                var emailKontrol = _kullanıcıRepository.GetAll().Any(x => x.Email == dto.Email);

                if (emailKontrol)
                {
                    return ResponseGeneric<Kullanıcı>.Error("Bu email adresi zaten kullanılıyor.");
                }

                var tcKontrol = _kullanıcıRepository.GetAll().Any(x => x.TCNo == dto.TCNo);
                if (tcKontrol)
                {
                    return ResponseGeneric<Kullanıcı>.Error("Bu TC No zaten kullanılıyor.");
                }

                if (dto.Rol == "Doktor" && !dto.UzmanlıkId.HasValue)
                {
                    return ResponseGeneric<Kullanıcı>.Error("Doktor için uzmanlık alanı zorunludur.");
                }

                var yeniKullanıcı = new Kullanıcı
                {
                    İsim = dto.İsim,
                    Soyisim = dto.Soyisim,
                    TCNo = dto.TCNo,
                    Email = dto.Email,
                    Parola = HashPassword(dto.Parola),
                    Rol = dto.Rol,
                    DoğumTarihi = dto.DoğumTarihi,
                    UzmanlıkId = dto.UzmanlıkId,
                    TelefonNumarası = dto.TelefonNumarası,
                    OluşturulmaTarihi = DateTime.Now
                };

                _kullanıcıRepository.Create(yeniKullanıcı);

                yeniKullanıcı.Parola = null;

                return ResponseGeneric<Kullanıcı>.Success(yeniKullanıcı, "Kullanıcı başarıyla oluşturuldu.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<Kullanıcı>.Error("Bir hata oluştu. " + ex.Message);
            }
        }

        public ResponseGeneric<Kullanıcı> KullanıcıGuncelle(int id, KullanıcıOlusturDto dto)
        {
            try
            {
                var kullanıcı = _kullanıcıRepository.GetById(id);
                if (kullanıcı == null)
                {
                    return ResponseGeneric<Kullanıcı>.Error("Kullanıcı bulunamadı.");
                }

                // Validasyonlar
                if (string.IsNullOrEmpty(dto.İsim) || string.IsNullOrEmpty(dto.Soyisim))
                {
                    return ResponseGeneric<Kullanıcı>.Error("İsim ve soyisim boş olamaz.");
                }

                if (string.IsNullOrEmpty(dto.Rol))
                {
                    return ResponseGeneric<Kullanıcı>.Error("Rol bilgisi boş olamaz.");
                }

                var izinliRoller = new[] { "Admin", "Doktor", "Hasta" };
                if (!izinliRoller.Contains(dto.Rol))
                {
                    return ResponseGeneric<Kullanıcı>.Error("Geçersiz rol. İzin verilen roller: Admin, Doktor, Hasta");
                }

                if (string.IsNullOrEmpty(dto.TCNo))
                {
                    return ResponseGeneric<Kullanıcı>.Error("TC No boş olamaz.");
                }

                if (dto.TCNo.Length != 11 || !dto.TCNo.All(char.IsDigit))
                {
                    return ResponseGeneric<Kullanıcı>.Error("TC No 11 haneli sayısal değer olmalıdır.");
                }

                // Email kontrolü (kendi emaili hariç)
                var emailKontrol = _kullanıcıRepository.GetAll().Any(x => x.Email == dto.Email && x.Id != id);
                if (emailKontrol)
                {
                    return ResponseGeneric<Kullanıcı>.Error("Bu email adresi başka bir kullanıcı tarafından kullanılıyor.");
                }

                // TC kontrolü (kendi TC'si hariç)
                var tcKontrol = _kullanıcıRepository.GetAll().Any(x => x.TCNo == dto.TCNo && x.Id != id);
                if (tcKontrol)
                {
                    return ResponseGeneric<Kullanıcı>.Error("Bu TC No başka bir kullanıcı tarafından kullanılıyor.");
                }

                // Güncelleme
                kullanıcı.İsim = dto.İsim;
                kullanıcı.Soyisim = dto.Soyisim;
                kullanıcı.TCNo = dto.TCNo;
                kullanıcı.Email = dto.Email;
                kullanıcı.Rol = dto.Rol;
                kullanıcı.DoğumTarihi = dto.DoğumTarihi;
                kullanıcı.TelefonNumarası = dto.TelefonNumarası;
                kullanıcı.UzmanlıkId = dto.UzmanlıkId;

                // Şifre sadece dolu ise güncellenir
                if (!string.IsNullOrEmpty(dto.Parola))
                {
                    kullanıcı.Parola = HashPassword(dto.Parola);
                }

                _kullanıcıRepository.Update(kullanıcı);

                kullanıcı.Parola = null;

                return ResponseGeneric<Kullanıcı>.Success(kullanıcı, "Kullanıcı başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<Kullanıcı>.Error("Bir hata oluştu. " + ex.Message);
            }
        }

        public ResponseGeneric<Kullanıcı> EmailGuncelle(int id, string yeniEmail)
        {
            try
            {
                if (string.IsNullOrEmpty(yeniEmail))
                {
                    return ResponseGeneric<Kullanıcı>.Error("Yeni email adresi boş olamaz.");
                }

                var kullanıcı = _kullanıcıRepository.GetById(id);

                if (kullanıcı == null)
                {
                    return ResponseGeneric<Kullanıcı>.Error("Girilen id'ye ait kullanıcı bulunamadı.");
                }

                var emailKontrol = _kullanıcıRepository.GetAll().Any(x => x.Email == yeniEmail && x.Id != id);

                if (emailKontrol)
                {
                    return ResponseGeneric<Kullanıcı>.Error("Bu email adresi başka bir kullanıcı tarafından kullanılıyor.");
                }

                kullanıcı.Email = yeniEmail;
                _kullanıcıRepository.Update(kullanıcı);

                kullanıcı.Parola = null;

                return ResponseGeneric<Kullanıcı>.Success(kullanıcı,"Email adresi başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<Kullanıcı>.Error("Bir hata oluştu. " + ex.Message);
            }
        }

        public ResponseGeneric<Kullanıcı> KullanıcıGetirByEmail(string email)
        {
            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    return ResponseGeneric<Kullanıcı>.Error("Email adresi boş olamaz.");
                }

                var kullanıcı = _kullanıcıRepository.GetAll().FirstOrDefault(k => k.Email == email);

                if (kullanıcı == null)
                {
                    return ResponseGeneric<Kullanıcı>.Error("Kullanıcı bulunamadı.");
                }

                kullanıcı.Parola = null;
                return ResponseGeneric<Kullanıcı>.Success(kullanıcı, "Kullanıcı bulundu.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<Kullanıcı>.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public ResponseGeneric<Kullanıcı> KullanıcıGetirById(int id)
        {
            try
            {
                var kullanıcı = _kullanıcıRepository.GetById(id);
                if (kullanıcı == null)
                {
                    return ResponseGeneric<Kullanıcı>.Error("Kullanıcı bulunamadı.");
                }
                kullanıcı.Parola = null;

                return ResponseGeneric<Kullanıcı>.Success(kullanıcı, "Kullanıcı başarıyla getirildi");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Kullanıcı>.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public Responses KullanıcıSil(int id)
        {
            try
            {
                var kullanıcı = _kullanıcıRepository.GetById(id);
                if (kullanıcı == null)
                {
                    return Responses.Error("Girilen id'ye ait kullanıcı bulunamadı.");
                }
                
                _kullanıcıRepository.Delete(kullanıcı);
                return Responses.Success("Kullanıcı başarıyla silindi");
            }
            catch(Exception ex)
            {
                return Responses.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public Responses ParolaGuncelle(int kullanıcıId, string eskiParola, string yeniParola)
        {
            try
            {
                if (string.IsNullOrEmpty(eskiParola) || string.IsNullOrEmpty(yeniParola))
                {
                    return Responses.Error("Eski ve yeni parola boş olamaz.");
                }

                var kullanıcı = _kullanıcıRepository.GetById(kullanıcıId);

                if (kullanıcı == null)
                {
                    return Responses.Error("Kullanıcı bulunamadı.");
                }

                if (!VerifyPassword(eskiParola, kullanıcı.Parola))
                {
                    return Responses.Error("Mevcut parolanız hatalı.");
                }

                kullanıcı.Parola = HashPassword(yeniParola);
                _kullanıcıRepository.Update(kullanıcı);

                return Responses.Success("Parolanız başarıyla değiştirildi.");
            }
            catch (Exception ex)
            {
                return Responses.Error("Bir hata oluştu. " + ex.Message);
            }
        }

        public ResponseGeneric<List<Kullanıcı>> TumKullanıcılarıGetir()
        {
            try
            {
                var kullanıcılar = _kullanıcıRepository.GetAll().ToList();
                if (kullanıcılar.Count == 0)
                {
                    return ResponseGeneric<List<Kullanıcı>>.Error("Kullanıcı bulunamadı.");
                }
                foreach (var item in kullanıcılar)
                {
                    item.Parola = null;
                }
                return ResponseGeneric<List<Kullanıcı>>.Success(kullanıcılar, "Kullanıcılar başarıyla getirildi.");

            }
            catch(Exception ex)
            {
                return ResponseGeneric<List<Kullanıcı>>.Error("Bir hata oluştu." + ex.Message);

            }
        }

        public ResponseGeneric<Kullanıcı> HastaKayıt(KullanıcıKayıtDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Parola))
                {
                    return ResponseGeneric<Kullanıcı>.Error("Email ve parola boş olamaz.");
                }

                if (string.IsNullOrEmpty(dto.İsim) || string.IsNullOrEmpty(dto.Soyisim))
                {
                    return ResponseGeneric<Kullanıcı>.Error("İsim ve soyisim boş olamaz.");
                }

                if (string.IsNullOrEmpty(dto.TCNo))
                {
                    return ResponseGeneric<Kullanıcı>.Error("TC No boş olamaz.");
                }

                if (dto.TCNo.Length != 11 || !dto.TCNo.All(char.IsDigit))
                {
                    return ResponseGeneric<Kullanıcı>.Error("TC No 11 haneli sayısal değer olmalıdır.");
                }

                var emailKontrol = _kullanıcıRepository.GetAll().Any(x => x.Email == dto.Email);
                if (emailKontrol)
                {
                    return ResponseGeneric<Kullanıcı>.Error("Bu email adresi zaten kullanılıyor.");
                }

                var tcKontrol = _kullanıcıRepository.GetAll().Any(x => x.TCNo == dto.TCNo);
                if (tcKontrol)
                {
                    return ResponseGeneric<Kullanıcı>.Error("Bu TC No zaten kullanılıyor.");
                }

                var yeniKullanici = new Kullanıcı
                {
                    İsim = dto.İsim,
                    Soyisim = dto.Soyisim,
                    TCNo = dto.TCNo,
                    Email = dto.Email,
                    Parola = HashPassword(dto.Parola),
                    Rol = "Hasta",
                    DoğumTarihi = dto.DoğumTarihi,
                    TelefonNumarası = dto.TelefonNumarası,
                    OluşturulmaTarihi = DateTime.Now
                };

                _kullanıcıRepository.Create(yeniKullanici);

                // Hasta kaydını da oluştur (Kullanıcı Id'si ile ilişkilendir)
                var yeniHasta = new Hasta
                {
                    KullanıcıId = yeniKullanici.Id,
                    Cinsiyet = dto.Cinsiyet ?? "",
                    RecordDate = DateTime.Now
                };
                _hastaRepository.Create(yeniHasta);

                yeniKullanici.Parola = null;

                return ResponseGeneric<Kullanıcı>.Success(yeniKullanici, "Kayıt başarıyla tamamlandı.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<Kullanıcı>.Error("Bir hata oluştu. " + ex.Message);
            }
        }
        public ResponseGeneric<LoginResponseDto> Login(KullanıcıGirisDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Email) || string.IsNullOrEmpty(dto.Parola))
                {
                    return ResponseGeneric<LoginResponseDto>.Error("Email ve parola boş olamaz.");
                }

                var kullanıcı = _kullanıcıRepository.GetAll().FirstOrDefault(u => u.Email == dto.Email);
                if (kullanıcı == null)
                {
                    return ResponseGeneric<LoginResponseDto>.Error("Email veya parola hatalı.");
                }

                if (!VerifyPassword(dto.Parola, kullanıcı.Parola))
                {
                    return ResponseGeneric<LoginResponseDto>.Error("Email veya parola hatalı.");
                }

                var generatedToken = GenerateJwtToken(kullanıcı);

                var loginResponse = new LoginResponseDto
                {
                    Token = generatedToken,
                    Expiration = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryInMinutes"])),
                    KullanıcıId = kullanıcı.Id,
                    Email = kullanıcı.Email,
                    İsim = kullanıcı.İsim,
                    Soyisim = kullanıcı.Soyisim,
                    Rol = kullanıcı.Rol
                };

                return ResponseGeneric<LoginResponseDto>.Success(loginResponse, "Giriş işlemi başarılıyla tamamlandı.");
            }

            catch (Exception ex)
            {
                return ResponseGeneric<LoginResponseDto>.Error("Bir hata oluştu. " + ex.Message);
            }
        }

        private string HashPassword(string password)
        {
            string secretKey = _configuration["Security:PasswordHashSecretKey"];
               
            using (var sha256 = SHA256.Create())
            {
                var combinedPassword = password + secretKey;
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(combinedPassword));
                var hashedPassword = Convert.ToBase64String(bytes);
                return hashedPassword;
            }
        }

        private bool VerifyPassword(string girilenParola, string kayıtlıHashedParola)
        {
            string girilenHash = HashPassword(girilenParola);
            
            return girilenHash == kayıtlıHashedParola;
        }

        private string GenerateJwtToken(Kullanıcı kullanıcı)
        {
            var claims = new[]
                {
        new Claim(ClaimTypes.NameIdentifier, kullanıcı.Id.ToString()),
        new Claim(ClaimTypes.Email, kullanıcı.Email),
        new Claim(ClaimTypes.Name, $"{kullanıcı.İsim} {kullanıcı.Soyisim}"),
        new Claim(ClaimTypes.Role, kullanıcı.Rol),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryInMinutes"]));

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
