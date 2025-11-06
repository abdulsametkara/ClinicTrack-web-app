using ClinickCore.DTOs;
using ClinickService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;

namespace ClinickTrackApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HastaController : ControllerBase
    {
        private readonly IHastaService _hastaService;
        public HastaController(IHastaService hastaService)
        {
            _hastaService = hastaService;
        }

        [HttpGet("getAll")]
        public IActionResult TumHastalariGetir()
        {
            var sonuc = _hastaService.TumHastalariGetir();
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpGet("getById/{id}")]
        public IActionResult HastaGetirById(int id)
        {
            var sonuc = _hastaService.HastaGetirById(id);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpPost("add")]
        public IActionResult HastaEkle([FromBody] HastaOlusturDto dto)
        {
            var sonuc = _hastaService.HastaEkle(dto);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpPut("update/{id}")]
        public IActionResult HastaGüncelle(int id, [FromBody] HastaGüncelleDto dto)
        {
            var sonuc = _hastaService.HastaGuncelle(id, dto);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpDelete("delete/{id}")]
        public IActionResult HastaSil(int id)
        {
            var sonuc = _hastaService.HastaSil(id);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }
    }
}
