using ClinickCore.DTOs;
using ClinickService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinickTrackApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RandevuController : ControllerBase
    {
        private readonly IRandevuService _randevuService;
        public RandevuController(IRandevuService randevuService)
        {
            _randevuService = randevuService;
        }

        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize]
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

        [Authorize(Roles = "Admin,Doktor")]
        [HttpPut("updateStatus/{id}")]
        public IActionResult RandevuDurumGuncelle(int id, [FromBody] string yeniDurum)
        {
            var sonuc = _randevuService.RandevuDurumGuncelle(id, yeniDurum);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [Authorize(Roles = "Doktor")]
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

        [Authorize]
        [HttpGet("checkAvailability")]
        public IActionResult RandevuUygunMu(int doktorId, DateTime randevuTarihi)
        {
            var sonuc = _randevuService.RandevuUygunMu(doktorId, randevuTarihi);
            return Ok(
                new { 
                    uygunMu = sonuc, message = sonuc ? "Randevu uygun" : "Randevu uygun değil" 
                });
        }


        [Authorize(Roles = "Admin")]
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

        [Authorize]
        [HttpGet("getAvailableSlots")]
        public IActionResult GetMusaitRandevuSaatleri(int doktorId, DateTime tarih)
        {
            var sonuc = _randevuService.GetMusaitRandevuSaatleri(doktorId, tarih);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }
    }
}
