using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickCore.DTOs
{
    public class KullanıcıOlusturDto
    {
        public string İsim { get; set; }
        public string Soyisim { get; set; }
        public string TCNo { get; set; }
        public string Email { get; set; }
        public string Parola { get; set; }
        public string Rol { get; set; }
        public DateTime DoğumTarihi { get; set; }
        public int? UzmanlıkId { get; set; }
        public string TelefonNumarası { get; set; }
    }

    public class KullanıcıGirisDto
    {
        public string Email { get; set; }
        public string Parola { get; set; }
    }

    public class KullanıcıKayıtDto
    {
        public string İsim { get; set; }
        public string Soyisim { get; set; }
        public string TCNo { get; set; }
        public string Email { get; set; }
        public string Parola { get; set; }
        public DateTime DoğumTarihi { get; set; }
        public string Cinsiyet { get; set; }
        public string TelefonNumarası { get; set; }

    }

    public class KullanıcıGüncelleDto
    {
        public string Email { get; set; }
        public string Parola { get; set; }
    }

    public class ParolaGuncelleDto
    {
        public string EskiParola { get; set; }
        public string YeniParola { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public int KullanıcıId { get; set; }
        public string Email { get; set; }
        public string İsim { get; set; }
        public string Soyisim { get; set; }
        public string Rol { get; set; }
    }

}
