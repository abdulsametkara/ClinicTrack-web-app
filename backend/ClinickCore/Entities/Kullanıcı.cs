using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickCore.Entities
{
    public class Kullanıcı : BaseEntity
    {
        public string İsim { get; set; }
        public string Soyisim { get; set; }
        public string TCNo { get; set; }
        public string Email { get; set; }
        public string Parola { get; set; }
        public string Rol { get; set; }
        public DateTime? DoğumTarihi { get; set; }
        public int? UzmanlıkId { get; set; }
        public string? TelefonNumarası { get; set; }
        public DateTime? OluşturulmaTarihi { get; set; }
        public bool İlkGiris { get; set; } = true; // Varsayılan olarak true
    }
}
