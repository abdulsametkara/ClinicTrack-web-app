using ClinickCore;
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
    public interface IKullanıcıService
    {
        ResponseGeneric<Kullanıcı> KullanıcıEkle(KullanıcıOlusturDto kullanıcı);
        ResponseGeneric<List<Kullanıcı>> TumKullanıcılarıGetir();
        ResponseGeneric<Kullanıcı> KullanıcıGetirById(int id);
        ResponseGeneric<Kullanıcı> KullanıcıGuncelle(int id, KullanıcıGüncelleDto kullanıcı);
        Responses KullanıcıSil(int id);

    }
}
