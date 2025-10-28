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

        public ResponseGeneric<List<Hasta>> TumHastalariGetir()
        {
            var hastalar = _hastaRepository.GetAll().ToList();
            return ResponseGeneric<List<Hasta>>.Success(hastalar, "Tüm hastalar başarıyla getirildi.");
        }

        public ResponseGeneric<Hasta> HastaGetirById(int id)
        {
            var hasta = _hastaRepository.GetById(id);
            if (hasta == null)
            {
                return ResponseGeneric<Hasta>.Error("Belirtilen ID'ye sahip hasta bulunamadı.");
            }
            return ResponseGeneric<Hasta>.Success(hasta, "Hasta başarıyla getirildi.");
        }

        public Responses HastaSil(int id)
        {
            var hasta = _hastaRepository.GetById(id);
            if (hasta == null)
            {
                return Responses.Error("Belirtilen ID'ye sahip hasta bulunamadı.");
            }
            _hastaRepository.Delete(hasta);
            return Responses.Success("Hasta başarıyla silindi.");
        }


        ResponseGeneric<Hasta> IHastaService.HastaGuncelle(int id, HastaGüncelleDto hasta)
        {
            if(hasta == null)
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
            }
            if (mevcutHasta.Email != hasta.Email)
            {
                var emailKontrol = _hastaRepository.GetAll().FirstOrDefault(h => h.Email == hasta.Email);
                if (emailKontrol != null)
                {
                    return ResponseGeneric<Hasta>.Error("Bu email adresine sahip bir hasta zaten mevcut.");
                }
            }
            _hastaRepository.Update(mevcutHasta);
            return ResponseGeneric<Hasta>.Success(mevcutHasta, "Hasta başarıyla güncellendi.");
        }
    }
}
