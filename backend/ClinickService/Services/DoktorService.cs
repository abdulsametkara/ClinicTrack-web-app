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
        private readonly IGenericRepository<Kullanıcı> _kullanıcıRepository;
        private readonly IGenericRepository<Uzmanlık> _uzmanlıkRepository;
        private readonly IGenericRepository<Randevu> _randevuRepository;
        
        public DoktorService(
            IGenericRepository<Doktor> doktorRepository,
            IGenericRepository<Kullanıcı> kullanıcıRepository,
            IGenericRepository<Uzmanlık> uzmanlıkRepository,
            IGenericRepository<Randevu> randevuRepository
            )
        {
            _doktorRepository = doktorRepository;
            _kullanıcıRepository = kullanıcıRepository;
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

                var emailKontrol = _kullanıcıRepository.GetAll().Any(u => u.Email == doktor.Email);
                if (emailKontrol)
                {
                    return ResponseGeneric<Doktor>.Error("Bu email adresi zaten kullanılıyor.");
                }

                var telKontrol = _kullanıcıRepository.GetAll().Any(u => u.TelefonNumarası == doktor.TelefonNumarası);
                if (telKontrol)
                {
                    return ResponseGeneric<Doktor>.Error("Bu telefon numarası zaten kullanılıyor.");
                }

                var yeniKullanıcı = new Kullanıcı
                {
                    İsim = doktor.İsim,
                    Soyisim = doktor.Soyisim,
                    TCNo = doktor.TCNo,
                    Email = doktor.Email,
                    Parola = "DoktorSifre123",
                    Rol = "Doktor",
                    UzmanlıkId = doktor.UzmanlıkId,
                    TelefonNumarası = doktor.TelefonNumarası,
                    OluşturulmaTarihi = DateTime.Now,
                    RecordDate = DateTime.Now
                };
                _kullanıcıRepository.Create(yeniKullanıcı);

                var yeniDoktor = new Doktor
                {
                    KullanıcıId = yeniKullanıcı.Id,
                    UzmanlıkId = doktor.UzmanlıkId,
                    DiplomaNo = doktor.DiplomaNo,
                    MezuniyetTarihi = doktor.MezuniyetTarihi,
                    Ünvan = doktor.Ünvan,
                    RecordDate = DateTime.Now
                };
                _doktorRepository.Create(yeniDoktor);
                
                return ResponseGeneric<Doktor>.Success(yeniDoktor, "Doktor başarıyla eklendi.");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Doktor>.Error("Bir hata oluştu. " + ex.Message);
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

                var kullanıcı = _kullanıcıRepository.GetById(kayıtlıDoktor.KullanıcıId);
                if (kullanıcı == null)
                {
                    return ResponseGeneric<Doktor>.Error("Doktor kullanıcı bilgisi bulunamadı.");
                }

                if (doktor.UzmanlıkId.HasValue && doktor.UzmanlıkId != kayıtlıDoktor.UzmanlıkId)
                {
                    var uzmanlık = _uzmanlıkRepository.GetById(doktor.UzmanlıkId.Value);
                    if (uzmanlık == null)
                    {
                        return ResponseGeneric<Doktor>.Error("Uzmanlık bulunamadı.");
                    }
                    kayıtlıDoktor.UzmanlıkId = doktor.UzmanlıkId.Value;
                    kullanıcı.UzmanlıkId = doktor.UzmanlıkId.Value;
                }

                if (!string.IsNullOrEmpty(doktor.Email) && doktor.Email != kullanıcı.Email)
                {
                    var emailKontrol = _kullanıcıRepository.GetAll().Any(u => u.Email == doktor.Email && u.Id != kullanıcı.Id);
                    if (emailKontrol)
                    {
                        return ResponseGeneric<Doktor>.Error("Email adresi başkası tarafından alınmış.");
                    }
                    kullanıcı.Email = doktor.Email;
                }

                if (!string.IsNullOrEmpty(doktor.TelefonNumarası) && doktor.TelefonNumarası != kullanıcı.TelefonNumarası)
                {
                    var telKontrol = _kullanıcıRepository.GetAll().Any(u => u.TelefonNumarası == doktor.TelefonNumarası && u.Id != kullanıcı.Id);
                    if (telKontrol)
                    {
                        return ResponseGeneric<Doktor>.Error("Telefon numarası başkası tarafından alınmış.");
                    }
                    kullanıcı.TelefonNumarası = doktor.TelefonNumarası;
                }

                _kullanıcıRepository.Update(kullanıcı);

                if (!string.IsNullOrEmpty(doktor.DiplomaNo))
                    kayıtlıDoktor.DiplomaNo = doktor.DiplomaNo;
                
                if (doktor.MezuniyetTarihi.HasValue)
                    kayıtlıDoktor.MezuniyetTarihi = doktor.MezuniyetTarihi;
                
                if (!string.IsNullOrEmpty(doktor.Ünvan))
                    kayıtlıDoktor.Ünvan = doktor.Ünvan;

                _doktorRepository.Update(kayıtlıDoktor);
                return ResponseGeneric<Doktor>.Success(kayıtlıDoktor, "Doktor bilgileri başarıyla güncellendi");
            }
            catch(Exception ex)
            {
                return ResponseGeneric<Doktor>.Error("Bir hata oluştu. " + ex.Message);
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
