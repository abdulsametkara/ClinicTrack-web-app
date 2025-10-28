using ClinickCore;
using ClinickCore.Entities;
using ClinickService.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickService.Interfaces
{
    public interface IRandevuService
    {
        ResponseGeneric<Randevu> RandevuEkle(int hastaId, int doktorId, DateTime randevuTarihi);
        Responses RandevuIptal(int randevuId);
        ResponseGeneric<List<Randevu>> HastaRandevularınıGetir(int hastaId);
        ResponseGeneric<List<Randevu>> DoktorRandevularınıGetir(int doktorId);
        ResponseGeneric<Randevu> RandevuGetirById(int randevuId);
        ResponseGeneric<List<Randevu>> TümRandevularıGetir();
        bool RandevuUygunMu(int doktorId, DateTime randevuTarihi);
        ResponseGeneric<Randevu> RandevuDurumGuncelle(int randevuId, string yeniDurum);
        Responses DoktorNotEkle(int randevuId, string not);
        Responses GeçmişRandevularıTamamla();
    }
}
