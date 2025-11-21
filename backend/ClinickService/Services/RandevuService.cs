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
        private readonly IGenericRepository<Kullanıcı> _kullanıcıRepository;

        public RandevuService(
            IGenericRepository<Doktor> doktorRepository,
            IGenericRepository<Hasta> hastaRepository,
            IGenericRepository<Uzmanlık> uzmanlıkRepository,
            IGenericRepository<Randevu> randevuRepository,
            IGenericRepository<Kullanıcı> kullanıcıRepository
            )
        {
            _doktorRepository = doktorRepository;
            _hastaRepository = hastaRepository;
            _uzmanlıkRepository = uzmanlıkRepository;
            _randevuRepository = randevuRepository;
            _kullanıcıRepository = kullanıcıRepository;
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

                // İsimleri doldur
                var hastaIds = randevular.Select(r => r.HastaId).Distinct().ToList();
                var hastalar = _hastaRepository.GetAll().Where(h => hastaIds.Contains(h.Id)).ToList();
                
                var kullaniciIds = hastalar.Select(h => h.KullanıcıId).Distinct().ToList();
                var kullanicilar = _kullanıcıRepository.GetAll().Where(k => kullaniciIds.Contains(k.Id)).ToList();

                foreach(var r in randevular)
                {
                    var hasta = hastalar.FirstOrDefault(h => h.Id == r.HastaId);
                    if(hasta != null)
                    {
                         var kullanici = kullanicilar.FirstOrDefault(k => k.Id == hasta.KullanıcıId);
                         if(kullanici != null)
                         {
                             r.HastaAdi = kullanici.İsim;
                             r.HastaSoyadi = kullanici.Soyisim;
                             r.HastaTCNo = kullanici.TCNo;
                             r.HastaTelefon = kullanici.TelefonNumarası;
                             r.HastaEmail = kullanici.Email;
                         }
                    }
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
                var suAn = DateTime.Now;
                Console.WriteLine($"[GEÇMİŞ RANDEVU] Şu an: {suAn}");
                
                var tumRandevular = _randevuRepository.GetAll().ToList();
                Console.WriteLine($"[GEÇMİŞ RANDEVU] Toplam randevu sayısı: {tumRandevular.Count}");
                
                var gecmisRandevular = tumRandevular.Where(x => x.RandevuTarihi < suAn && x.Durum == "Beklemede").ToList();
                
                Console.WriteLine($"[GEÇMİŞ RANDEVU] Geçmiş ve beklemede olan: {gecmisRandevular.Count}");
                foreach (var r in gecmisRandevular)
                {
                    Console.WriteLine($"  - ID: {r.Id}, Tarih: {r.RandevuTarihi}, Durum: {r.Durum}");
                }
                
                if (gecmisRandevular.Count == 0)
                {
                    return Responses.Success("Geçmiş ve tamamlanması gereken randevu bulunamadı.");
                }
                
                foreach (var randevu in gecmisRandevular)
                {
                    randevu.Durum = "Tamamlandı";
                    _randevuRepository.Update(randevu);
                    Console.WriteLine($"[GEÇMİŞ RANDEVU] Randevu ID {randevu.Id} tamamlandı olarak işaretlendi.");
                }

                return Responses.Success($"{gecmisRandevular.Count} adet geçmiş randevu otomatik olarak tamamlandı.");
            }
            catch(Exception ex)
            {
                Console.WriteLine($"[GEÇMİŞ RANDEVU HATA] {ex.Message}");
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
                // Doktor ve Uzmanlık isimlerini doldur
                var doktorIds = randevular.Select(r => r.DoktorId).Distinct().ToList();
                var doktorlar = _doktorRepository.GetAll().Where(d => doktorIds.Contains(d.Id)).ToList();
                var doktorKullaniciIds = doktorlar.Select(d => d.KullanıcıId).Distinct().ToList();
                var doktorKullanicilar = _kullanıcıRepository.GetAll().Where(k => doktorKullaniciIds.Contains(k.Id)).ToList();

                var uzmanlikIds = doktorlar.Select(d => d.UzmanlıkId).Distinct().ToList();
                var uzmanliklar = _uzmanlıkRepository.GetAll().Where(u => uzmanlikIds.Contains(u.Id)).ToList();


                foreach (var r in randevular)
                {
                     // Hasta bilgileri zaten yukarıda dolduruldu veya doldurulmalı...
                     // (Aynı mantıkla doktor bilgilerini dolduralım)
                     
                     var doktor = doktorlar.FirstOrDefault(d => d.Id == r.DoktorId);
                     if (doktor != null)
                     {
                         var k = doktorKullanicilar.FirstOrDefault(u => u.Id == doktor.KullanıcıId);
                         if (k != null)
                         {
                             r.DoktorAdi = k.İsim;
                             r.DoktorSoyadi = k.Soyisim;
                         }
                         
                         var u = uzmanliklar.FirstOrDefault(uz => uz.Id == doktor.UzmanlıkId);
                         if (u != null)
                         {
                             r.UzmanlıkAdi = u.UzmanlıkAdı;
                         }
                     }
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
        public ResponseGeneric<List<string>> GetMusaitRandevuSaatleri(int doktorId, DateTime tarih)
        {
            try
            {
                // Hafta sonu kontrolü
                if (tarih.DayOfWeek == DayOfWeek.Saturday || tarih.DayOfWeek == DayOfWeek.Sunday)
                {
                    return ResponseGeneric<List<string>>.Error("Hafta sonları randevu alınamaz.");
                }

                var baslangicSaati = new TimeSpan(9, 0, 0);
                var bitisSaati = new TimeSpan(17, 0, 0);
                var suAn = DateTime.Now;

                var doluRandevular = _randevuRepository.GetAll()
                    .Where(r => r.DoktorId == doktorId && 
                           r.RandevuTarihi.Date == tarih.Date && 
                           r.Durum != "İptal")
                    .Select(r => r.RandevuTarihi.TimeOfDay)
                    .ToList();

                var musaitSaatler = new List<string>();

                for (var saat = baslangicSaati; saat < bitisSaati; saat = saat.Add(TimeSpan.FromHours(1)))
                {
                    // Geçmiş saat kontrolü (Eğer seçilen tarih bugün ise)
                    if (tarih.Date == suAn.Date && saat < suAn.TimeOfDay)
                    {
                        continue;
                    }

                    if (!doluRandevular.Contains(saat))
                    {
                        musaitSaatler.Add(saat.ToString(@"hh\:mm"));
                    }
                }

                return ResponseGeneric<List<string>>.Success(musaitSaatler, "Müsait saatler getirildi.");
            }
            catch (Exception ex)
            {
                return ResponseGeneric<List<string>>.Error("Bir hata oluştu: " + ex.Message);
            }
        }
    }
}
