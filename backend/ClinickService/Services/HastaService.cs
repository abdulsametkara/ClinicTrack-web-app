using ClinickCore;
using ClinickCore.DTOs;
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
        public HastaService(IGenericRepository<Hasta> hastaRepository)
        {
            _hastaRepository = hastaRepository;
        }
        public ResponseGeneric<Hasta> HastaEkle(HastaOlusturDto hasta)
        {
            try
            {
                if (hasta == null)
                {
                    return ResponseGeneric<Hasta>.Error("Hasta bilgileri boş olamaz.");
                }

                var tcKontrol = _hastaRepository.GetAll().FirstOrDefault(h => h.TCNo == hasta.TCNo);
                if (tcKontrol != null)
                {
                    return ResponseGeneric<Hasta>.Error("Bu TC kimlik numarasına sahip bir hasta zaten mevcut.");
                }

                var emailKontrol = _hastaRepository.GetAll().FirstOrDefault(h => h.Email == hasta.Email);
                if (emailKontrol != null)
                {
                    return ResponseGeneric<Hasta>.Error("Bu email adresine sahip bir hasta zaten mevcut.");
                }

                var yeniHasta = new Hasta
                {
                    İsim = hasta.İsim,
                    Soyisim = hasta.Soyisim,
                    TCNo = hasta.TCNo,
                    DoğumTarihi = hasta.DoğumTarihi,
                    Cinsiyet = hasta.Cinsiyet,
                    TelefonNumarası = hasta.TelefonNumarası,
                    Email = hasta.Email,
                    RecordDate = DateTime.Now
                };
                _hastaRepository.Create(yeniHasta);
                return ResponseGeneric<Hasta>.Success(yeniHasta, "Hasta başarıyla eklendi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Hasta>.Error("Bir hata oluştu." + ex.Message);
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

                if (mevcutHasta.TelefonNumarası != hasta.TelefonNumarası)
                {
                    var telKontrol = _hastaRepository.GetAll().FirstOrDefault(h => h.TelefonNumarası == hasta.TelefonNumarası);
                    if (telKontrol != null)
                    {
                        return ResponseGeneric<Hasta>.Error("Bu telefon numarasına sahip bir hasta zaten mevcut.");
                    }
                    mevcutHasta.TelefonNumarası = hasta.TelefonNumarası;
                }

                if (mevcutHasta.Email != hasta.Email)
                {
                    var emailKontrol = _hastaRepository.GetAll().FirstOrDefault(h => h.Email == hasta.Email);
                    if (emailKontrol != null)
                    {
                        return ResponseGeneric<Hasta>.Error("Bu email adresine sahip bir hasta zaten mevcut.");
                    }
                    mevcutHasta.Email = hasta.Email;
                }

                _hastaRepository.Update(mevcutHasta);
                return ResponseGeneric<Hasta>.Success(mevcutHasta, "Hasta başarıyla güncellendi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Hasta>.Error("Bir hata oluştu." + ex.Message);
            }
        }
    }
}
