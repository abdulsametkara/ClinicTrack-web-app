using ClinickCore.Entities;
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
        ResponseGeneric<Uzmanlık> UzmanlıkEkle(string uzmanlıkAdı);
        ResponseGeneric<List<Uzmanlık>> TumUzmanlıklarıGetir();
        ResponseGeneric<Uzmanlık> UzmanlıkGetirById(int id);
        ResponseGeneric<Uzmanlık> UzmanlıkGuncelle(int id, string uzmanlıkAdı);
        Responses UzmanlıkSil(int id);
    }
}
