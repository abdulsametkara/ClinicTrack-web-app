using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickCore.DTOs
{
    public class HastaOlusturDto
    {
        public string İsim { get; set; }
        public string Soyisim { get; set; }
        public string TCNo { get; set; }
        public DateTime DoğumTarihi { get; set; }
        public string Cinsiyet { get; set; }
        public string TelefonNumarası { get; set; }
        public string Email { get; set; }
    }

    public class HastaGüncelleDto
    {
        public string TelefonNumarası { get; set; }
        public string Email { get; set; }
    }
}
