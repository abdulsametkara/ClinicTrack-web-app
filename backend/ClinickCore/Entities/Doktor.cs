using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;

namespace ClinickCore.Entities
{
    public class Doktor : BaseEntity
    {
        public int KullanıcıId { get; set; }
        public int UzmanlıkId { get; set; }
        public string? Ünvan { get; set; }

        [NotMapped]
        public string? İsim { get; set; }
        [NotMapped]
        public string? Soyisim { get; set; }
        [NotMapped]
        public string? UzmanlıkAdi { get; set; }
    }
}
