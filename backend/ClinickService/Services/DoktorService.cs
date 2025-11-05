using ClinickCore;
using ClinickCore.DTOs;
using ClinickCore.Entities;
using ClinickDataAccess.Repository;
using ClinickService.Interfaces;
using ClinickService.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ClinickService.Services
{
    public class DoktorService : IDoktorService
    {
        private readonly IGenericRepository<Doktor> _doktorRepository;
        private readonly IGenericRepository<Uzmanlık> _uzmanlıkRepository;
        private readonly IGenericRepository<Randevu> _randevuRepository;
        public DoktorService(
            IGenericRepository<Doktor> doktorRepository,
            IGenericRepository<Uzmanlık> uzmanlıkRepository,
            IGenericRepository<Randevu> randevuRepository
            )
        {
            _doktorRepository = doktorRepository;
            _uzmanlıkRepository = uzmanlıkRepository;
            _randevuRepository = randevuRepository;
        }
        public ResponseGeneric<Doktor> DoktorEkle(DoktorOlusturDto doktor)
        {
            try
            {
                if (doktor == null)
                {
                    return ResponseGeneric<Doktor>.Error("Doktor bilgileri boş olamaz");
                }

                var uzmanlık = _uzmanlıkRepository.GetById(doktor.UzmanlıkId);
                if (uzmanlık == null)
                {
                    return ResponseGeneric<Doktor>.Error("Belirtilen uzmanlık bulunamadı.");
                }

                var emailKontrol = _doktorRepository.GetAll().FirstOrDefault(x => x.Email == doktor.Email);
                if (emailKontrol != null)
                {
                    return ResponseGeneric<Doktor>.Error("Bu email adresi zaten kullanılıyor.");
                }

                var telKontrol = _doktorRepository.GetAll().FirstOrDefault(x => x.TelefonNumarası == doktor.TelefonNumarası);
                if (telKontrol != null)
                {
                    return ResponseGeneric<Doktor>.Error("Bu telefon numarası zaten kullanılıyor.");
                }

                var yeniDoktor = new Doktor
                {
                    İsim = doktor.İsim,
                    Soyisim = doktor.Soyisim,
                    UzmanlıkId = doktor.UzmanlıkId,
                    TelefonNumarası = doktor.TelefonNumarası,
                    Email = doktor.Email,
                    RecordDate = DateTime.Now
                };
                _doktorRepository.Create(yeniDoktor);
                return ResponseGeneric<Doktor>.Success(yeniDoktor, "Doktor başarıyla eklendi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Doktor>.Error("Bir hata oluştu." + ex.Message);
            }
        }


        public ResponseGeneric<Doktor> DoktorGetirById(int id)
        {
            try
            {
                var doktor = _doktorRepository.GetById(id);

                if (doktor == null)
                {
                    return ResponseGeneric<Doktor>.Error("Girilen id'ye ait doktor bulunamadı.");
                }

                return ResponseGeneric<Doktor>.Success(doktor, "Doktor başarıyla bulundu.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Doktor>.Error("Bir hata oluştu." + ex.Message);
            }
        }


        public ResponseGeneric<List<Doktor>> DoktorGetirUzmanlığaGore(int uzmanlıkId)
        {
            try
            {
                var uzmanlık = _uzmanlıkRepository.GetById(uzmanlıkId);
                if (uzmanlık == null)
                {
                    return ResponseGeneric<List<Doktor>>.Error("Girilen id'ye ait uzmanlık bulunamadı.");
                }

                var doktor = _doktorRepository.GetAll().Where(x => x.UzmanlıkId == uzmanlıkId).ToList();
                if (doktor.Count == 0)
                {
                    return ResponseGeneric<List<Doktor>>.Error("Uzmanlığa ait doktor bulunamadı.");
                }
                return ResponseGeneric<List<Doktor>>.Success(doktor, "Doktorlar başarıyla getirildi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<List<Doktor>>.Error("Bir hata oluştu." + ex.Message);
            }
        }


        public ResponseGeneric<Doktor> DoktorGuncelle(int id, DoktorGüncelleDto doktor)
        {
            try
            {
                if (doktor == null)
                {
                    return ResponseGeneric<Doktor>.Error("Doktor bilgileri boş bırakılamaz.");
                }

                var kayıtlıDoktor = _doktorRepository.GetById(id);
                if (kayıtlıDoktor == null)
                {
                    return ResponseGeneric<Doktor>.Error("Girilen id'li doktor bulunamadı.");
                }

                if (doktor.UzmanlıkId.HasValue && doktor.UzmanlıkId != kayıtlıDoktor.UzmanlıkId)
                {
                    var uzmanlık = _uzmanlıkRepository.GetById(doktor.UzmanlıkId.Value);
                    if (uzmanlık == null)
                    {
                        return ResponseGeneric<Doktor>.Error("Uzmanlık bulunamadı.");
                    }
                    kayıtlıDoktor.UzmanlıkId = doktor.UzmanlıkId.Value;
                }

                if (doktor.Email != null && doktor.Email != kayıtlıDoktor.Email)
                {
                    var emailKontrol = _doktorRepository.GetAll().FirstOrDefault(x => x.Email == doktor.Email && x.Id != id);
                    if (emailKontrol != null)
                    {
                        return ResponseGeneric<Doktor>.Error("Email adresi başkası tarafından alınmış.");
                    }
                    kayıtlıDoktor.Email = doktor.Email;
                }

                if (doktor.TelefonNumarası != null && doktor.TelefonNumarası != kayıtlıDoktor.TelefonNumarası)
                {
                    var telKontrol = _doktorRepository.GetAll().FirstOrDefault(x => x.TelefonNumarası == doktor.TelefonNumarası && x.Id != id);
                    if (telKontrol != null)
                    {
                        return ResponseGeneric<Doktor>.Error("Telefon numarası başkası tarafından alınmış.");
                    }
                    kayıtlıDoktor.TelefonNumarası = doktor.TelefonNumarası;
                }
                _doktorRepository.Update(kayıtlıDoktor);
                return ResponseGeneric<Doktor>.Success(kayıtlıDoktor, "Doktor bilgileri başarıyla güncellendi");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Doktor>.Error("Bir hata oluştu." + ex.Message);
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

                var randevu = _randevuRepository.GetAll().Where(x => x.DoktorId == doktorId).ToList();
                if (randevu.Count == 0)
                {
                    return ResponseGeneric<List<Randevu>>.Error("Doktora ait randevu bulunamadı");
                }

                return ResponseGeneric<List<Randevu>>.Success(randevu, "Randevular getirildi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<List<Randevu>>.Error("Bir hata oluştu." + ex.Message);
            }
        }


        public Responses DoktorSil(int id)
        {
            try
            {
                var doktor = _doktorRepository.GetById(id);
                if (doktor == null)
                {
                    return Responses.Error("Girilen id'ye ait doktor bulunamadı.");
                }
                _doktorRepository.Delete(doktor);
                return Responses.Success("Doktor başarıyla silindi.");
            }
            catch(Exception ex)
            {
                return Responses.Error("Bir hata oluştu." + ex.Message);
            }
        }

        public ResponseGeneric<List<Doktor>> TumDoktolarıGetir()
        {
            try
            {
                var doktor = _doktorRepository.GetAll().ToList();
                if (doktor.Count == 0)
                {
                    return ResponseGeneric<List<Doktor>>.Error("Kayıtlı doktorlar bulunamadı.");
                }
                return ResponseGeneric<List<Doktor>>.Success(doktor, "Kayıtlı doktorlar getirildi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<List<Doktor>>.Error("Bir hata oluştu." + ex.Message);
            }
        }
    }
}
