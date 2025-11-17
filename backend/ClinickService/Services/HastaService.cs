using ClinickCore.DTOs;
using ClinickCore.Entities;
using ClinickDataAccess.Repository;
using ClinickService.Interfaces;
using ClinickService.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickService.Services
{
    public class HastaService : IHastaService
    {
        private readonly IGenericRepository<Hasta> _hastaRepository;
        private readonly IGenericRepository<Kullanıcı> _kullanıcıRepository;
        
        public HastaService(IGenericRepository<Hasta> hastaRepository, IGenericRepository<Kullanıcı> kullanıcıRepository)
        {
            _hastaRepository = hastaRepository;
            _kullanıcıRepository = kullanıcıRepository;
        }
        public ResponseGeneric<Hasta> HastaEkle(HastaOlusturDto hasta)
        {
            try
            {
                if (hasta == null)
                {
                    return ResponseGeneric<Hasta>.Error("Hasta bilgileri boş olamaz.");
                }

                var tcKontrol = _kullanıcıRepository.GetAll().Any(u => u.TCNo == hasta.TCNo);
                if (tcKontrol)
                {
                    return ResponseGeneric<Hasta>.Error("Bu TC kimlik numarasına sahip bir kullanıcı zaten mevcut.");
                }

                var emailKontrol = _kullanıcıRepository.GetAll().Any(u => u.Email == hasta.Email);
                if (emailKontrol)
                {
                    return ResponseGeneric<Hasta>.Error("Bu email adresine sahip bir kullanıcı zaten mevcut.");
                }

                var yeniKullanıcı = new Kullanıcı
                {
                    İsim = hasta.İsim,
                    Soyisim = hasta.Soyisim,
                    TCNo = hasta.TCNo,
                    Email = hasta.Email,
                    Parola = "Sifre123",
                    Rol = "Hasta",
                    DoğumTarihi = hasta.DoğumTarihi,
                    TelefonNumarası = hasta.TelefonNumarası,
                    OluşturulmaTarihi = DateTime.Now,
                    RecordDate = DateTime.Now
                };
                _kullanıcıRepository.Create(yeniKullanıcı);

                var yeniHasta = new Hasta
                {
                    KullanıcıId = yeniKullanıcı.Id,
                    Cinsiyet = hasta.Cinsiyet,
                    KanGrubu = hasta.KanGrubu,
                    Adres = hasta.Adres,
                    RecordDate = DateTime.Now
                };
                _hastaRepository.Create(yeniHasta);
                
                return ResponseGeneric<Hasta>.Success(yeniHasta, "Hasta başarıyla eklendi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Hasta>.Error("Bir hata oluştu. " + ex.Message);
            }
        }

        public ResponseGeneric<List<Hasta>> TumHastalariGetir()
        {
            try
            {
                var hastalar = _hastaRepository.GetAll().ToList();
                if (hastalar.Count == 0)
                {
                    return ResponseGeneric<List<Hasta>>.Error("Kayıtlı hasta bulunamadı.");
                }
                return ResponseGeneric<List<Hasta>>.Success(hastalar, "Tüm hastalar başarıyla getirildi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<List<Hasta>>.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public ResponseGeneric<Hasta> HastaGetirById(int id)
        {
            try
            {
                var hasta = _hastaRepository.GetById(id);
                if (hasta == null)
                {
                    return ResponseGeneric<Hasta>.Error("Belirtilen ID'ye sahip hasta bulunamadı.");
                }
                return ResponseGeneric<Hasta>.Success(hasta, "Hasta başarıyla getirildi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Hasta>.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public ResponseGeneric<Hasta> HastaGetirByKullanıcıId(int kullanıcıId)
        {
            try
            {
                var hasta = _hastaRepository.GetAll().FirstOrDefault(h => h.KullanıcıId == kullanıcıId);
                if (hasta == null)
                {
                    return ResponseGeneric<Hasta>.Error("Kullanıcıya ait hasta kaydı bulunamadı.");
                }
                return ResponseGeneric<Hasta>.Success(hasta, "Hasta bilgileri başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<Hasta>.Error("Bir hata oluştu: " + ex.Message);
            }
        }

        public Responses HastaSil(int id)
        {
            try
            {
                var hasta = _hastaRepository.GetById(id);
                if (hasta == null)
                {
                    return Responses.Error("Belirtilen ID'ye sahip hasta bulunamadı.");
                }
                _hastaRepository.Delete(hasta);
                return Responses.Success("Hasta başarıyla silindi.");
            }
            catch(Exception ex)
            {
                return Responses.Error("Bir hata oluştu." + ex.Message);
            }
        }


        ResponseGeneric<Hasta> IHastaService.HastaGuncelle(int id, HastaGüncelleDto hasta)
        {
            try
            {
                if (hasta == null)
                {
                    return ResponseGeneric<Hasta>.Error("Hasta bilgileri boş olamaz.");
                }

                var mevcutHasta = _hastaRepository.GetById(id);
                if (mevcutHasta == null)
                {
                    return ResponseGeneric<Hasta>.Error("Belirtilen ID'ye sahip hasta bulunamadı.");
                }

                var kullanıcı = _kullanıcıRepository.GetById(mevcutHasta.KullanıcıId);
                if (kullanıcı == null)
                {
                    return ResponseGeneric<Hasta>.Error("Hasta kullanıcı bilgisi bulunamadı.");
                }

                if (!string.IsNullOrEmpty(hasta.TelefonNumarası) && kullanıcı.TelefonNumarası != hasta.TelefonNumarası)
                {
                    var telKontrol = _kullanıcıRepository.GetAll().Any(u => u.TelefonNumarası == hasta.TelefonNumarası && u.Id != kullanıcı.Id);
                    if (telKontrol)
                    {
                        return ResponseGeneric<Hasta>.Error("Bu telefon numarasına sahip bir kullanıcı zaten mevcut.");
                    }
                    kullanıcı.TelefonNumarası = hasta.TelefonNumarası;
                }

                if (!string.IsNullOrEmpty(hasta.Email) && kullanıcı.Email != hasta.Email)
                {
                    var emailKontrol = _kullanıcıRepository.GetAll().Any(u => u.Email == hasta.Email && u.Id != kullanıcı.Id);
                    if (emailKontrol)
                    {
                        return ResponseGeneric<Hasta>.Error("Bu email adresine sahip bir kullanıcı zaten mevcut.");
                    }
                    kullanıcı.Email = hasta.Email;
                }

                _kullanıcıRepository.Update(kullanıcı);

                if (!string.IsNullOrEmpty(hasta.Cinsiyet))
                    mevcutHasta.Cinsiyet = hasta.Cinsiyet;
                
                if (!string.IsNullOrEmpty(hasta.KanGrubu))
                    mevcutHasta.KanGrubu = hasta.KanGrubu;
                
                if (!string.IsNullOrEmpty(hasta.Adres))
                    mevcutHasta.Adres = hasta.Adres;

                _hastaRepository.Update(mevcutHasta);
                return ResponseGeneric<Hasta>.Success(mevcutHasta, "Hasta başarıyla güncellendi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Hasta>.Error("Bir hata oluştu. " + ex.Message);
            }
        }
    }
}
