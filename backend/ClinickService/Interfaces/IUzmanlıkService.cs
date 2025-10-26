using ClinickCore;
using ClinickService.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickService.Interfaces
{
    public interface IUzmanlıkService
    {
        IResponse UzmanlıkEkle(string uzmanlıkAdı);
        ResponseGeneric<List<Uzmanlık>> TumUzmanlıklarıGetir();
        ResponseGeneric<Uzmanlık> UzmanlıkGetirById(int id);
        IResponse UzmanlıkGuncelle(int id, string uzmanlıkAdı);
        IResponse UzmanlıkSil(int id);
    }
}
