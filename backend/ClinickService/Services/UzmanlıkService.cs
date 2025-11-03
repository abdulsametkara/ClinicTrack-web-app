using ClinickCore.Entities;
using ClinickDataAccess.Repository;
using ClinickService.Interfaces;
using ClinickService.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace ClinickService.Services
{
    public class UzmanlıkService : IUzmanlıkService
    {
        private readonly IGenericRepository<Uzmanlık> uzmanlıkRepository;
        public UzmanlıkService(IGenericRepository<Uzmanlık> uzmanlıkService)
        {
            uzmanlıkRepository = uzmanlıkService;
        }


        public ResponseGeneric<List<Uzmanlık>> TumUzmanlıklarıGetir()
        {
            var uzmanlık = uzmanlıkRepository.GetAll().ToList();
            if (uzmanlık.Count == 0)
            {
                return ResponseGeneric<List<Uzmanlık>>.Error("Uzmanlıklar bulunamadı.");
            }
            return ResponseGeneric<List<Uzmanlık>>.Success(uzmanlık, "Uzmanlıklar başarıyla getirildi.");
        }



        public ResponseGeneric<Uzmanlık> UzmanlıkEkle(string uzmanlıkAdı)
        {
            if (string.IsNullOrWhiteSpace(uzmanlıkAdı))
            {
                return ResponseGeneric<Uzmanlık>.Error("Uzmanlık adı boş bırakılamaz.");
            }

            var mevcutUzmanlık = uzmanlıkRepository.GetAll().FirstOrDefault(x => x.UzmanlıkAdı.ToLower() == uzmanlıkAdı.ToLower());
            if (mevcutUzmanlık != null)
            {
                return ResponseGeneric<Uzmanlık>.Error("Uzmanlık adı zaten mevcut.");
            }

            var yeniUzmanlık = new Uzmanlık
            {
                UzmanlıkAdı = uzmanlıkAdı.Trim(),
                RecordDate = DateTime.Now
            };
            uzmanlıkRepository.Create(yeniUzmanlık);
            return ResponseGeneric<Uzmanlık>.Success(yeniUzmanlık, "Yeni uzmanlık başarıyla oluşturuldu.");
            
        }

        public ResponseGeneric<Uzmanlık> UzmanlıkGetirById(int id)
        {
            var uzmanlık = uzmanlıkRepository.GetById(id);
            if (uzmanlık == null)
            {
                return ResponseGeneric<Uzmanlık>.Error("Girilen id'ye ait uzmanlık bulunamadı");
            }
            return ResponseGeneric<Uzmanlık>.Success(uzmanlık, "Uzmanlık bulundu.");
        }

        public ResponseGeneric<Uzmanlık> UzmanlıkGuncelle(int id, string uzmanlıkAdı)
        {
            if (string.IsNullOrWhiteSpace(uzmanlıkAdı))
            {
                return ResponseGeneric<Uzmanlık>.Error("Uzmanlık adı boş bırakılamaz.");
            }

            var uzmanlık = uzmanlıkRepository.GetById(id);
            if (uzmanlık == null)
            {
                return ResponseGeneric<Uzmanlık>.Error("Girilen id'ye ait uzmanlık bulunamadı.");
            }

            var mevcutUzmanlık = uzmanlıkRepository.GetAll().FirstOrDefault(x => x.UzmanlıkAdı.ToLower() == uzmanlıkAdı.ToLower() && x.Id != id);
            if (mevcutUzmanlık != null)
            {
                return ResponseGeneric<Uzmanlık>.Error("Uzmanlık adı zaten mevcut.");
            }
            uzmanlık.UzmanlıkAdı = uzmanlıkAdı.Trim();

            uzmanlıkRepository.Update(uzmanlık);
            return ResponseGeneric<Uzmanlık>.Success(uzmanlık, "Uzmanlık başarıyla güncellendi.");
        }

        public Responses UzmanlıkSil(int id)
        {
            var uzmanlık = uzmanlıkRepository.GetById(id);
            if (uzmanlık == null)
            {
                return Responses.Error("Uzmanlık bulunamadı.");
            }
            uzmanlıkRepository.Delete(uzmanlık);
            return Responses.Success("Uzmanlık başarıyla silindi.");
        }
    }
}
