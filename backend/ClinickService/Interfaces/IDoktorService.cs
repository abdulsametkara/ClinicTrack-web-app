using ClinickCore.DTOs;
using ClinickCore.Entities;
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
        ResponseGeneric<Doktor> DoktorEkle(DoktorOlusturDto doktor);
        ResponseGeneric<List<Doktor>> TumDoktolarıGetir();
        ResponseGeneric<Doktor> DoktorGetirById(int id);
        ResponseGeneric<Doktor> DoktorGuncelle(int id, DoktorGüncelleDto doktor);
        Responses DoktorSil(int id);
        ResponseGeneric<List<Doktor>> DoktorGetirUzmanlığaGore(int uzmanlıkId);
        ResponseGeneric<List<Randevu>> DoktorRandevularınıGetir(int doktorId);

    }
}
