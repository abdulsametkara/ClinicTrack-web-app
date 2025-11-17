using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickCore.Entities
{
    public class Hasta : BaseEntity
    {
        public int KullanıcıId { get; set; }
        public string? Cinsiyet { get; set; }
        public string? KanGrubu { get; set; }
        public string? Adres { get; set; }
        public string? AcilDurumKişisi { get; set; }
        public string? AcilDurumTelefon { get; set; }
    }
}
