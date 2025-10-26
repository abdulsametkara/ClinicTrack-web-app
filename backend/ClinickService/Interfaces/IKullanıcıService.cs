using ClinickCore;
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
        IResponse KullanıcıEkle(Kullanıcı kullanıcı);
        ResponseGeneric<List<Kullanıcı>> TumKullanıcılarıGetir();
        ResponseGeneric<Kullanıcı> KullanıcıGetirById(int id);
        IResponse KullanıcıGuncelle(int id, Kullanıcı kullanıcı);
        IResponse KullanıcıSil(int id);

    }
}
