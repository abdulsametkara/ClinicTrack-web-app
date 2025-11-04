using ClinickCore;
using ClinickCore.Entities;
using ClinickDataAccess.Repository;
using ClinickService.Interfaces;
using ClinickService.Response;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClinickService.Services
{
    public class RandevuService : IRandevuService
    {
        private readonly IGenericRepository<Doktor> _doktorRepository;
        private readonly IGenericRepository<Hasta> _hastaRepository;
        private readonly IGenericRepository<Uzmanlık> _uzmanlıkRepository;
        private readonly IGenericRepository<Randevu> _randevuRepository;
        public RandevuService(
            IGenericRepository<Doktor> doktorRepository,
            IGenericRepository<Hasta> hastaRepository,
            IGenericRepository<Uzmanlık> uzmanlıkRepository,
            IGenericRepository<Randevu> randevuRepository
            )
        {
            _doktorRepository = doktorRepository;
            _hastaRepository = hastaRepository;
            _uzmanlıkRepository = uzmanlıkRepository;
            _randevuRepository = randevuRepository;
        }
        public Responses DoktorNotEkle(int randevuId, string not)
        {
            var randevu = _randevuRepository.GetById(randevuId);
            if (randevu == null)
            {
                return Responses.Error("Girilen id'ye ait randevu bulunamadı.");
            }
            if (string.IsNullOrWhiteSpace(not))
            {
                return Responses.Error("Not boş olamaz");
            }
            randevu.DoktorNotları = not.Trim();
            _randevuRepository.Update(randevu);
            return Responses.Success("Randevu notu başarıyla eklendi.");
        }

        public ResponseGeneric<List<Randevu>> DoktorRandevularınıGetir(int doktorId)
        {
            var doktor = _doktorRepository.GetById(doktorId);
            if (doktor == null)
            {
                return ResponseGeneric<List<Randevu>>.Error("Girilen id'ye ait doktor bulunamadı.");
            }
            var randevular = _randevuRepository.GetAll().Where(x => x.DoktorId == doktorId).OrderByDescending(x => x.RandevuTarihi).ToList();
            if (randevular.Count == 0)
            {
                return ResponseGeneric<List<Randevu>>.Error("Doktora ait randevu bulunamadı.");
            }
            return ResponseGeneric<List<Randevu>>.Success(randevular, "Doktora ait randevular başarıyla getirildi");
        }

        public Responses GeçmişRandevularıTamamla()
        {
            var gecmisRandevular = _randevuRepository.GetAll().Where(x => x.RandevuTarihi < DateTime.Now && x.Durum == "Beklemede").ToList();
            if (gecmisRandevular.Count == 0)
            {
                return Responses.Error("Geçmiş ve tamamlanması gereken randevu bulunamadı.");
            }
            foreach (var randevu in gecmisRandevular)
            {
                randevu.Durum = "Tamamlandı";
                _randevuRepository.Update(randevu);
            }

            return Responses.Success("Geçmiş randevu durumları otomatik olarak güncellendi.");
        }

        public ResponseGeneric<List<Randevu>> HastaRandevularınıGetir(int hastaId)  
        {
            var hasta = _hastaRepository.GetById(hastaId);
            if (hasta == null)
            {
                return ResponseGeneric<List<Randevu>>.Error("Girilen id'ye ait hasta bulunamadı");
            }

            var randevular = _randevuRepository.GetAll().Where(x => x.HastaId == hastaId).OrderByDescending(x => x.RandevuTarihi).ToList();
            if (randevular.Count == 0)
            {
                return ResponseGeneric<List<Randevu>>.Error("Hastaya ait randevu bulunamadı");
            }
            return ResponseGeneric<List<Randevu>>.Success(randevular, "Hastaya ait randevular başarıyla getirildi.");

        }

        public ResponseGeneric<Randevu> RandevuDurumGuncelle(int randevuId, string yeniDurum)
        {
            var randevu = _randevuRepository.GetById(randevuId);
            if (randevu == null)
            {
                return ResponseGeneric<Randevu>.Error("Girilen id'ye ait randevu bulunamadı.");
            }
            if (yeniDurum != "Beklemede" && yeniDurum != "Tamamlandı" && yeniDurum != "İptal")
            {
                return ResponseGeneric<Randevu>.Error("Girilen randevu durumu geçerli değildir. Tamamlandı, Beklemede veya İptal durumları geçerlidir.");
            }
            randevu.Durum = yeniDurum;
            _randevuRepository.Update(randevu);
            return ResponseGeneric<Randevu>.Success(randevu, "Randevu durumu başarıyla gerçekleşti.");
        }

        public ResponseGeneric<Randevu> RandevuEkle(int hastaId, int doktorId, DateTime randevuTarihi)
        {
            if (randevuTarihi < DateTime.Now)
            {
                return ResponseGeneric<Randevu>.Error("Seçtiğiniz tarihe randevu alamazsınız.");
            }

            var hasta = _hastaRepository.GetById(hastaId);
            if (hasta == null)
            {
                return ResponseGeneric<Randevu>.Error("Girdiğiniz id'ye ait hasta bulunamadı");
            }

            var doktor = _doktorRepository.GetById(doktorId);
            if (doktor == null)
            {
                return ResponseGeneric<Randevu>.Error("Girdiğiniz id'ye ait doktor bulunamadı");
            }

            var uzmanlık = _uzmanlıkRepository.GetById(doktor.UzmanlıkId);
            if (uzmanlık == null)
            {
                return ResponseGeneric<Randevu>.Error("Doktor uzmanlığı bulunamadı");
            }

            if (!RandevuUygunMu(doktorId, randevuTarihi))
            {
                return ResponseGeneric<Randevu>.Error("Seçilen doktorun bu tarih ve saatte randevusu bulunmaktadır.");
            }

            var yeniRandevu = new Randevu()
            {
                HastaId = hastaId,
                DoktorId = doktorId,
                UzmanlıkId = doktor.UzmanlıkId,
                RandevuTarihi = randevuTarihi,
                Durum = "Beklemede", //ilk durumu
                DoktorNotları = "",
                HastaŞikayeti = "",
                RecordDate = DateTime.Now
            };

            _randevuRepository.Create(yeniRandevu);
            return ResponseGeneric<Randevu>.Success(yeniRandevu, "Randevu başarıyla oluşturuldu");
        }

        public ResponseGeneric<Randevu> RandevuGetirById(int randevuId)
        {
            var randevu = _randevuRepository.GetById(randevuId);
            if (randevu == null)
            {
                return ResponseGeneric<Randevu>.Error("Girilen id'ye ait randevu bulunamadı.");
            }
            return ResponseGeneric<Randevu>.Success(randevu, "Randevu başarıyla getirildi.");
        }

        public Responses RandevuIptal(int randevuId)
        {
            var randevu = _randevuRepository.GetById(randevuId);
            if (randevu == null)
            {
                return Responses.Error("Girilen id'ye ait randevu bulunamadı");
            }

            if (randevu.Durum == "İptal")
            {
                return Responses.Error("Randevu zaten iptal edilmiş");
            }

            if (randevu.Durum == "Tamamlandı")
            {
                return Responses.Error("Tamamlanmış randevu iptal edilemez");
            }

            randevu.Durum = "İptal";
            _randevuRepository.Update(randevu);
            return Responses.Success("Randevu başarıyla iptal edildi.");
        }

        public bool RandevuUygunMu(int doktorId, DateTime randevuTarihi)
        {
            var randevu = _randevuRepository.GetAll().FirstOrDefault(x => x.DoktorId == doktorId && x.RandevuTarihi == randevuTarihi && x.Durum != "İptal");

            return randevu == null;
        }

        public ResponseGeneric<List<Randevu>> TümRandevularıGetir()
        {
            var randevular = _randevuRepository.GetAll().ToList();
            if (randevular.Count == 0)
            {
                return ResponseGeneric<List<Randevu>>.Error("Kayıtlı randevu bulunamadı");
            }
            return ResponseGeneric<List<Randevu>>.Success(randevular, "Randevular başarıyla getirildi.");
        }
    }
}
