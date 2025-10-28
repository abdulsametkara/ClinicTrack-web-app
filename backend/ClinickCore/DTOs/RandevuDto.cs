using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickCore.DTOs
{
    public class RandevuOlusturDto
    {
        public int HastaId { get; set; }
        public int DoktorId { get; set; }
        public DateTime RandevuTarihi { get; set; }
        public string HastaŞikayeti { get; set; }
    }

    public class RandevuGüncelleDto
    {
        public int RandevuId { get; set; }
        public DateTime? RandevuTarihi { get; set; }
        public string? HastaŞikayeti { get; set; }
        public string? Durum { get; set; }
        public string? DoktorNotları { get; set; }
    }
}
