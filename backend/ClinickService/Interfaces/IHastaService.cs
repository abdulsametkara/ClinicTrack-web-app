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
    public interface IHastaService
    {
        ResponseGeneric<Hasta> HastaEkle(HastaOlusturDto hasta);
        ResponseGeneric<List<Hasta>> TumHastalariGetir();
        ResponseGeneric<Hasta> HastaGetirById(int id);
        ResponseGeneric<Hasta> HastaGetirByKullanıcıId(int kullanıcıId);
        ResponseGeneric<Hasta> HastaGuncelle(int id, HastaGüncelleDto hasta);
        Responses HastaSil(int id);

    }
}
