using ClinickCore;
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
        IResponse HastaEkle(Hasta hasta);
        ResponseGeneric<List<Hasta>> TumHastalariGetir();
        ResponseGeneric<Hasta> HastaGetirById(int id);
        IResponse HastaGuncelle(int id, Hasta hasta);
        IResponse HastaSil(int id);

    }
}
