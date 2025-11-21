using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations.Schema;

namespace ClinickCore.Entities
{
    public class Randevu : BaseEntity
    {
        public int HastaId { get; set; }
        public int DoktorId { get; set; }
        public int UzmanlıkId { get; set; }
        public DateTime RandevuTarihi { get; set; }
        public string Durum { get; set; }
        public string HastaŞikayeti { get; set; }
        public string DoktorNotları { get; set; }

        [NotMapped]
        public string? HastaAdi { get; set; }
        [NotMapped]
        public string? HastaSoyadi { get; set; }
        [NotMapped]
        public string? HastaTCNo { get; set; }
        [NotMapped]
        public string? HastaTelefon { get; set; }
        [NotMapped]
        public string? HastaEmail { get; set; }
        
        [NotMapped]
        public string? DoktorAdi { get; set; }
        [NotMapped]
        public string? DoktorSoyadi { get; set; }
        [NotMapped]
        public string? UzmanlıkAdi { get; set; }
    }
}
