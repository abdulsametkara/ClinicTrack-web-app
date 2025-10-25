using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickCore
{
    public class Doktor : BaseEntity
    {
        public string İsim { get; set; }
        public string Soyisim { get; set; }
        public string UzmanlıkAlanı { get; set; }
        public string TelefonNumarası { get; set; }
        public string Email { get; set; }
    }
}
