using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinickCore.DTOs
{
    public class UzmanlıkOlusturDto
    {
        public string UzmanlıkAdı { get; set; }
    }
    public class UzmanlıkGüncelleDto
    {
        public int UzmanlıkId { get; set; }
        public string UzmanlıkAdı { get; set; }
    }
}
