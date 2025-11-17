using ClinickCore.DTOs;
using ClinickService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinickTrackApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoktorController : ControllerBase
    {
        private readonly IDoktorService _doktorService;
        public DoktorController(IDoktorService doktorService)
        {
            _doktorService = doktorService;
        }

        [AllowAnonymous]
        [HttpGet("getAll")]
        public IActionResult TumDoktorlariGetir()
        {
            var sonuc = _doktorService.TumDoktolarıGetir();
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [AllowAnonymous]
        [HttpGet("getById/{id}")]
        public IActionResult DoktorGetirById(int id)
        {
            var sonuc = _doktorService.DoktorGetirById(id);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public IActionResult DoktorEkle([FromBody] DoktorOlusturDto dto)
        {
            var sonuc = _doktorService.DoktorEkle(dto);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public IActionResult DoktorGuncelle(int id, [FromBody] DoktorGüncelleDto dto)
        {
            var sonuc = _doktorService.DoktorGuncelle(id, dto);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public IActionResult DoktorSil(int id)
        {
            var sonuc = _doktorService.DoktorSil(id);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [AllowAnonymous]
        [HttpGet("uzmanlik/{uzmanlikId}")]
        public IActionResult DoktorGetirUzmanlığaGore(int uzmanlikId)
        {
            var sonuc = _doktorService.DoktorGetirUzmanlığaGore(uzmanlikId);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [Authorize(Roles = "Admin,Doktor")]
        [HttpGet("{doktorId}/randevular")]
        public IActionResult DoktorRandevularınıGetir(int doktorId)
        {
            var sonuc = _doktorService.DoktorRandevularınıGetir(doktorId);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [Authorize(Roles = "Doktor")]
        [HttpGet("profil")]
        public IActionResult ProfilDoktor()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("Kullanıcı bilgisi bulunamadı.");
            }
            
            var sonuc = _doktorService.DoktorGetirByKullanıcıId(int.Parse(currentUserId));
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }
    }
}
