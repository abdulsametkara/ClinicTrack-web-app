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

                if (hasta.KullanıcıId <= 0)
                {
                    return ResponseGeneric<Hasta>.Error("Geçerli bir kullanıcı seçilmelidir.");
                }

                var mevcutHasta = _hastaRepository.GetAll().FirstOrDefault(h => h.KullanıcıId == hasta.KullanıcıId);
                if (mevcutHasta != null)
                {
                    return ResponseGeneric<Hasta>.Error("Bu kullanıcıya ait bir hasta profili zaten mevcut.");
                }

                var yeniHasta = new Hasta
                {
                    KullanıcıId = hasta.KullanıcıId,
                    Cinsiyet = hasta.Cinsiyet,
                    KanGrubu = hasta.KanGrubu,
                    Adres = hasta.Adres,
                    AcilDurumKişisi = hasta.AcilDurumKişisi,
                    AcilDurumTelefon = hasta.AcilDurumTelefon,
                    RecordDate = DateTime.Now
                };
                _hastaRepository.Create(yeniHasta);
                return ResponseGeneric<Hasta>.Success(yeniHasta, "Hasta profili başarıyla oluşturuldu.");
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


        public ResponseGeneric<Hasta> HastaGetirByKullanıcıId(int kullanıcıId)
        {
            try
            {
                var hasta = _hastaRepository.GetAll().FirstOrDefault(h => h.KullanıcıId == kullanıcıId);
                if (hasta == null)
                {
                    return ResponseGeneric<Hasta>.Error("Belirtilen kullanıcıya ait hasta kaydı bulunamadı.");
                }

                return ResponseGeneric<Hasta>.Success(hasta, "Hasta bilgileri başarıyla getirildi.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<Hasta>.Error("Bir hata oluştu." + ex.Message);
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

                mevcutHasta.Cinsiyet = hasta.Cinsiyet ?? mevcutHasta.Cinsiyet;
                mevcutHasta.KanGrubu = hasta.KanGrubu ?? mevcutHasta.KanGrubu;
                mevcutHasta.Adres = hasta.Adres ?? mevcutHasta.Adres;
                mevcutHasta.AcilDurumKişisi = hasta.AcilDurumKişisi ?? mevcutHasta.AcilDurumKişisi;
                mevcutHasta.AcilDurumTelefon = hasta.AcilDurumTelefon ?? mevcutHasta.AcilDurumTelefon;

                _hastaRepository.Update(mevcutHasta);
                return ResponseGeneric<Hasta>.Success(mevcutHasta, "Hasta profili başarıyla güncellendi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Hasta>.Error("Bir hata oluştu." + ex.Message);
            }
        }
    }
}
