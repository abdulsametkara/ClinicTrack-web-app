using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickCore
{
    public class Randevu : BaseEntity
    {
        public int HastaId { get; set; }
        public int DoktorId { get; set; }
        public DateTime RandevuTarihi { get; set; }
        public string Durum { get; set; }
    }
}
