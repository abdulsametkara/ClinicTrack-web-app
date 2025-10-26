using ClinickCore;
using ClinickService.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickService.Interfaces
{
    public interface IDoktorService
    {
        IResponse DoktorEkle(Doktor doktor);
        ResponseGeneric<List<Doktor>> TumDoktolarıGetir();
        ResponseGeneric<Doktor> DoktorGetirById(int id);
        IResponse DoktorGuncelle(int id, Doktor doktor);
        IResponse DoktorSil(int id);
        ResponseGeneric<List<Doktor>> DoktorGetirUzmanlığaGore(int uzmanlıkId);
        ResponseGeneric<List<Randevu>> DoktorRandevularınıGetir(int doktorId);

    }
}
