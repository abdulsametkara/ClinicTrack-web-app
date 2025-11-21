using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickCore.DTOs
{
    public class KullanıcıOlusturDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("isim")]
        public string İsim { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("soyisim")]
        public string Soyisim { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("tcNo")]
        public string TCNo { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string Email { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("parola")]
        public string Parola { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("rol")]
        public string Rol { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("doğumTarihi")]
        public DateTime DoğumTarihi { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("uzmanlıkId")]
        public int? UzmanlıkId { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("telefonNumarası")]
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
        public bool İlkGiris { get; set; }
    }

    public class İlkParolaBelirleDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("email")]
        public string Email { get; set; }
        
        [System.Text.Json.Serialization.JsonPropertyName("yeniParola")]
        public string YeniParola { get; set; }
    }

}
