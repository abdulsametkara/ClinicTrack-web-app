using ClinickCore;
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
        IResponse RandevuEkle(int hastaId, int doktorId, DateTime randevuTarihi);
        IResponse RandevuIptal(int randevuId);
        ResponseGeneric<List<Randevu>> HastaRandevularınıGetir(int hastaId);
        ResponseGeneric<List<Randevu>> DoktorRandevularınıGetir(int doktorId);
        ResponseGeneric<Randevu> RandevuGetirById(int randevuId);
        ResponseGeneric<List<Randevu>> TümRandevularıGetir();
        bool RandevuUygunMu(int doktorId, DateTime randevuTarihi);
        IResponse RandevuDurumGuncelle(int randevuId, string yeniDurum);
        IResponse DoktorNotEkle(int randevuId, string not);
        IResponse GeçmişRandevularıTamamla();
    }
}
