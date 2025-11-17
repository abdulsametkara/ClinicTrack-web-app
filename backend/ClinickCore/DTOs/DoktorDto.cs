using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickCore.DTOs
{
    public class DoktorOlusturDto
    {
        public string İsim { get; set; }
        public string Soyisim { get; set; }
        public string TCNo { get; set; }
        public int UzmanlıkId { get; set; }
        public string TelefonNumarası { get; set; }
        public string Email { get; set; }
        public string? DiplomaNo { get; set; }
        public DateTime? MezuniyetTarihi { get; set; }
        public string? Ünvan { get; set; }
    }
    public class DoktorGüncelleDto
    {
        public int? UzmanlıkId { get; set; }
        public string? TelefonNumarası { get; set; }
        public string? Email { get; set; }
        public string? DiplomaNo { get; set; }
        public DateTime? MezuniyetTarihi { get; set; }
        public string? Ünvan { get; set; }
    }

}
