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
        ResponseGeneric<Kullanıcı> KullanıcıOlustur(KullanıcıOlusturDto dto);
        ResponseGeneric<Kullanıcı> HastaKayıt(KullanıcıKayıtDto dto);
        ResponseGeneric<List<Kullanıcı>> TumKullanıcılarıGetir();
        ResponseGeneric<Kullanıcı> KullanıcıGetirById(int id);
        ResponseGeneric<Kullanıcı> KullanıcıGetirByEmail(string email);
        ResponseGeneric<Kullanıcı> EmailGuncelle(int id, string yeniEmail);
        Responses KullanıcıSil(int id);
        ResponseGeneric<LoginResponseDto> Login(KullanıcıGirisDto dto);
        Responses ParolaGuncelle(int kullanıcıId, string eskiParola, string yeniParola);
    }
}
