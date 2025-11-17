using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickCore.Entities
{
    public class Doktor : BaseEntity
    {
        public int KullanıcıId { get; set; }
        public int UzmanlıkId { get; set; }
        public string? DiplomaNo { get; set; }
        public DateTime? MezuniyetTarihi { get; set; }
        public string? Ünvan { get; set; }
    }
}
