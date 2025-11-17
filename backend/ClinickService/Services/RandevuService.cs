using ClinickCore.DTOs;
using ClinickCore.Entities;
using ClinickDataAccess.Repository;
using ClinickService.Interfaces;
using ClinickService.Response;
using Microsoft.EntityFrameworkCore.Storage;
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
            try
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
            catch(Exception ex)
            {
                return Responses.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public ResponseGeneric<List<Randevu>> DoktorRandevularınıGetir(int doktorId)
        {
            try
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
            catch(Exception ex)
            {
                return ResponseGeneric<List<Randevu>>.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public Responses GeçmişRandevularıTamamla()
        {
            try
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
            catch(Exception ex)
            {
                return Responses.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public ResponseGeneric<List<Randevu>> HastaRandevularınıGetir(int hastaId)
        {
            try
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
            catch(Exception ex)
            {
                return ResponseGeneric<List<Randevu>>.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public ResponseGeneric<Randevu> RandevuDurumGuncelle(int randevuId, string yeniDurum)
        {
            try
            {
                var randevu = _randevuRepository.GetById(randevuId);
                if (randevu == null)
                {
                    return ResponseGeneric<Randevu>.Error("Girilen id'ye ait randevu bulunamadı.");
                }
                if (yeniDurum != "Beklemede" && yeniDurum != "Tamamlandı" && yeniDurum != "İptal")
                {
                    return ResponseGeneric<Randevu>.Error("Girilen randevu durumu geçerli değildir.");
                }
                randevu.Durum = yeniDurum;
                _randevuRepository.Update(randevu);
                return ResponseGeneric<Randevu>.Success(randevu, "Randevu durumu başarıyla güncellendi.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<Randevu>.Error($"Bir hata oluştu: {ex.Message}");
            }
        }

        public ResponseGeneric<Randevu> RandevuEkle(RandevuOlusturDto dto)
        {
            try
            {
                if (dto.RandevuTarihi < DateTime.Now)
                {
                    return ResponseGeneric<Randevu>.Error("Seçtiğiniz tarihe randevu alamazsınız.");
                }

                var hasta = _hastaRepository.GetById(dto.HastaId);
                if (hasta == null)
                {
                    return ResponseGeneric<Randevu>.Error("Girdiğiniz id'ye ait hasta bulunamadı");
                }

                var doktor = _doktorRepository.GetById(dto.DoktorId);
                if (doktor == null)
                {
                    return ResponseGeneric<Randevu>.Error("Girdiğiniz id'ye ait doktor bulunamadı");
                }

                var uzmanlık = _uzmanlıkRepository.GetById(doktor.UzmanlıkId);
                if (uzmanlık == null)
                {
                    return ResponseGeneric<Randevu>.Error("Doktor uzmanlığı bulunamadı");
                }

                if (!RandevuUygunMu(dto.DoktorId, dto.RandevuTarihi))
                {
                    return ResponseGeneric<Randevu>.Error("Seçilen doktorun bu tarih ve saatte randevusu bulunmaktadır.");
                }

                var yeniRandevu = new Randevu()
                {
                    HastaId = dto.HastaId,
                    DoktorId = dto.DoktorId,
                    UzmanlıkId = doktor.UzmanlıkId,
                    RandevuTarihi = dto.RandevuTarihi,
                    Durum = "Beklemede", //ilk durumu
                    DoktorNotları = "",
                    HastaŞikayeti = dto.HastaŞikayeti ?? "",
                    RecordDate = DateTime.Now
                };

                _randevuRepository.Create(yeniRandevu);
                return ResponseGeneric<Randevu>.Success(yeniRandevu, "Randevu başarıyla oluşturuldu");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Randevu>.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public ResponseGeneric<Randevu> RandevuGetirById(int randevuId)
        {
            try
            {
                var randevu = _randevuRepository.GetById(randevuId);
                if (randevu == null)
                {
                    return ResponseGeneric<Randevu>.Error("Girilen id'ye ait randevu bulunamadı.");
                }
                return ResponseGeneric<Randevu>.Success(randevu, "Randevu başarıyla getirildi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Randevu>.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public Responses RandevuIptal(int randevuId)
        {
            try
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
            catch(Exception ex)
            {
                return Responses.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public bool RandevuUygunMu(int doktorId, DateTime randevuTarihi)
        {
            var randevu = _randevuRepository.GetAll().FirstOrDefault(x => x.DoktorId == doktorId && x.RandevuTarihi == randevuTarihi && x.Durum != "İptal");

            return randevu == null;
        }

        public ResponseGeneric<List<Randevu>> TümRandevularıGetir()
        {
            try
            {
                var randevular = _randevuRepository.GetAll().ToList();
                if (randevular.Count == 0)
                {
                    return ResponseGeneric<List<Randevu>>.Error("Kayıtlı randevu bulunamadı");
                }
                return ResponseGeneric<List<Randevu>>.Success(randevular, "Randevular başarıyla getirildi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<List<Randevu>>.Error("Bir hata oluştu." + ex.Message);
            }
        }
        public Responses RandevuSil(int randevuId)
        {
            try
            {
                var randevu = _randevuRepository.GetById(randevuId);
                if (randevu == null)
                {
                    return Responses.Error("Randevu bulunamadı.");
                }

                _randevuRepository.Delete(randevu);
                return Responses.Success("Randevu veritabanından silindi.");
            }
            catch (Exception ex)
            {
                return Responses.Error("Bir hata oluştu." + ex.Message);
            }
        }
    }
}
