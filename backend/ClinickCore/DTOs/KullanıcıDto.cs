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
        public string Email { get; set; }
        public string Parola { get; set; }
        public string Rol { get; set; }
    }
    public class KullanıcıGüncelleDto
    {
        public int KullanıcıId { get; set; }
        public string Email { get; set; }
        public string ParolaHash { get; set; }
    }

}
