using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ClinickCore.DTOs;
using ClinickCore.Entities;
using ClinickDataAccess.Repository;
using ClinickService.Services;
using Moq;
using Xunit;

namespace Clinick.Tests.Services
{
    public class RandevuServiceDummyTests
    {
        [Fact]
        public void RandevuService_Tum_Methodler_Calisir()
        {
            var ctx = CreateServiceWithData();
            var service = ctx.service;
            var futureDate = NextWeekday(DateTime.Now.AddDays(1));

            service.DoktorNotEkle(1, "Kontrol notu");
            service.DoktorRandevularınıGetir(1);
            service.HastaRandevularınıGetir(1);
            service.RandevuDurumGuncelle(1, "Tamamlandı");
            service.RandevuGetirById(1);
            service.RandevuIptal(1);        // Tamamlandı durumu branch
            service.RandevuIptal(3);        // Başarılı iptal
            service.GeçmişRandevularıTamamla();
            service.TümRandevularıGetir();
            service.RandevuUygunMu(1, DateTime.Now.AddDays(5));

            var dto = new RandevuOlusturDto
            {
                HastaId = 1,
                DoktorId = 1,
                RandevuTarihi = futureDate.AddHours(1),
                HastaŞikayeti = "Baş ağrısı"
            };
            service.RandevuEkle(dto);

            service.RandevuGetirById(3);
            service.RandevuSil(3);
            service.GetMusaitRandevuSaatleri(1, futureDate);
        }

        private static (RandevuService service,
            List<Randevu> randevuList) CreateServiceWithData()
        {
            var doktorlar = new List<Doktor>
            {
                new Doktor { Id = 1, KullanıcıId = 101, UzmanlıkId = 201 }
            };

            var hastalar = new List<Hasta>
            {
                new Hasta { Id = 1, KullanıcıId = 301 }
            };

            var uzmanliklar = new List<Uzmanlık>
            {
                new Uzmanlık { Id = 201, UzmanlıkAdı = "Kardiyoloji" }
            };

            var kullanicilar = new List<Kullanıcı>
            {
                new Kullanıcı { Id = 101, İsim = "Dr", Soyisim = "Test", TCNo = "11111111111", Email = "dr@test.com", Rol = "Doktor" },
                new Kullanıcı { Id = 301, İsim = "Hasta", Soyisim = "Test", TCNo = "22222222222", Email = "hasta@test.com", Rol = "Hasta" }
            };

            var randevular = new List<Randevu>
            {
                new Randevu { Id = 1, HastaId = 1, DoktorId = 1, UzmanlıkId = 201, Durum = "Beklemede", RandevuTarihi = DateTime.Now.AddHours(2), HastaŞikayeti = "Deneme", DoktorNotları = "" },
                new Randevu { Id = 2, HastaId = 1, DoktorId = 1, UzmanlıkId = 201, Durum = "Beklemede", RandevuTarihi = DateTime.Now.AddHours(-4), HastaŞikayeti = "Geçmiş", DoktorNotları = "" },
                new Randevu { Id = 3, HastaId = 1, DoktorId = 1, UzmanlıkId = 201, Durum = "Beklemede", RandevuTarihi = DateTime.Now.AddHours(5), HastaŞikayeti = "Silinecek", DoktorNotları = "" }
            };

            var doktorRepo = CreateRepoMock(doktorlar, d => d.Id);
            var hastaRepo = CreateRepoMock(hastalar, h => h.Id);
            var uzmanRepo = CreateRepoMock(uzmanliklar, u => u.Id);
            var randevuRepo = CreateRepoMock(randevular, r => r.Id);
            var kullanıcıRepo = CreateRepoMock(kullanicilar, k => k.Id);

            var service = new RandevuService(
                doktorRepo.Object,
                hastaRepo.Object,
                uzmanRepo.Object,
                randevuRepo.Object,
                kullanıcıRepo.Object);

            return (service, randevular);
        }

        private static DateTime NextWeekday(DateTime date)
        {
            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }
            return date.Date.AddHours(10);
        }

        private static Mock<IGenericRepository<T>> CreateRepoMock<T>(List<T> store, Func<T, int> idSelector) where T : class
        {
            var mock = new Mock<IGenericRepository<T>>();

            mock.Setup(r => r.GetAll()).Returns(() => store.AsQueryable());

            mock.Setup(r => r.GetById(It.IsAny<int>()))
                .Returns((int id) => store.First(e => idSelector(e) == id));

            mock.Setup(r => r.Create(It.IsAny<T>()))
                .Callback<T>(entity =>
                {
                    var prop = entity.GetType().GetProperty("Id", BindingFlags.Instance | BindingFlags.Public);
                    if (prop != null && prop.PropertyType == typeof(int))
                    {
                        var currentId = (int)prop.GetValue(entity)!;
                        if (currentId == 0)
                        {
                            var nextId = store.Count == 0 ? 1 : store.Max(e => idSelector(e)) + 1;
                            prop.SetValue(entity, nextId);
                        }
                    }
                    store.Add(entity);
                });

            mock.Setup(r => r.Update(It.IsAny<T>()))
                .Callback<T>(entity =>
                {
                    var id = idSelector(entity);
                    var index = store.FindIndex(e => idSelector(e) == id);
                    if (index >= 0)
                    {
                        store[index] = entity;
                    }
                });

            mock.Setup(r => r.Delete(It.IsAny<T>()))
                .Callback<T>(entity =>
                {
                    var id = idSelector(entity);
                    store.RemoveAll(e => idSelector(e) == id);
                });

            return mock;
        }
    }
}

