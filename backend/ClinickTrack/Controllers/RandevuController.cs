using ClinickCore.DTOs;
using ClinickService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinickTrackApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RandevuController : ControllerBase
    {
        private readonly IRandevuService _randevuService;
        public RandevuController(IRandevuService randevuService)
        {
            _randevuService = randevuService;
        }

        [HttpGet("getAll")]
        public IActionResult TümRandevularıGetir()
        {
            var sonuc = _randevuService.TümRandevularıGetir();
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpDelete("delete/{id}")]
        public IActionResult RandevuSil(int id)
        {
            var sonuc = _randevuService.RandevuSil(id);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpPost("add")]
        public IActionResult RandevuEkle([FromBody] RandevuOlusturDto dto)
        {
            var sonuc = _randevuService.RandevuEkle(dto);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpPut("cancel/{id}")]
        public IActionResult RandevuIptal(int id)
        {
            var sonuc = _randevuService.RandevuIptal(id);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpGet("getById/{id}")]
        public IActionResult RandevuGetirById(int id)
        {
            var sonuc = _randevuService.RandevuGetirById(id);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpGet("hasta/{hastaId}")]
        public IActionResult HastaRandevularınıGetir(int hastaId)
        {
            var sonuc = _randevuService.HastaRandevularınıGetir(hastaId);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpGet("doktor/{doktorId}")]
        public IActionResult DoktorRandevularınıGetir(int doktorId)
        {
            var sonuc = _randevuService.DoktorRandevularınıGetir(doktorId);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpPut("updateStatus/{id}")]
        public IActionResult RandevuDurumGuncelle(int randevuId, [FromBody] string yeniDurum)
        {
            var sonuc = _randevuService.RandevuDurumGuncelle(randevuId, yeniDurum);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpPut("addNote/{id}")]
        public IActionResult DoktorNotEkle(int id, [FromBody] string not)
        {
            var sonuc = _randevuService.DoktorNotEkle(id, not);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpGet("checkAvailability")]
        public IActionResult RandevuUygunMu(int doktorId, DateTime randevuTarihi)
        {
            var sonuc = _randevuService.RandevuUygunMu(doktorId, randevuTarihi);
            return Ok(
                new { 
                    uygunMu = sonuc, message = sonuc ? "Randevu uygun" : "Randevu uygun değil" 
                });
        }


        [HttpPost("completePastAppointments")]
        public IActionResult GeçmişRandevularıTamamla()
        {
            var sonuc = _randevuService.GeçmişRandevularıTamamla();
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }


    }
}
