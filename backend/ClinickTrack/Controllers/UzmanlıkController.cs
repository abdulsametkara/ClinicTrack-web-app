using ClinickCore.DTOs;
using ClinickService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinickTrackApi.Controllers
{
    [Route("api/Uzmanlik")]
    [ApiController]
    [Authorize]
    public class UzmanlıkController : ControllerBase
    {
        private readonly IUzmanlıkService _uzmanlıkService;
        public UzmanlıkController(IUzmanlıkService uzmanlıkService)
        {
            _uzmanlıkService = uzmanlıkService;
        }

        [AllowAnonymous]
        [HttpGet("getAll")]
        public IActionResult TumUzmanlıklarıGetir()
        {
            var sonuc = _uzmanlıkService.TumUzmanlıklarıGetir();
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public IActionResult UzmanlıkEkle([FromBody] string uzmanlıkAdı)
        {
            var sonuc = _uzmanlıkService.UzmanlıkEkle(uzmanlıkAdı);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [AllowAnonymous]
        [HttpGet("getById/{id}")]
        public IActionResult UzmanlıkGetirById(int id)
        {
            var sonuc = _uzmanlıkService.UzmanlıkGetirById(id);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public IActionResult UzmanlıkGuncelle(int id, [FromBody] string uzmanlikAdı)
        {
            var sonuc = _uzmanlıkService.UzmanlıkGuncelle(id, uzmanlikAdı);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public IActionResult UzmanlıkSil(int id)
        {
            var sonuc = _uzmanlıkService.UzmanlıkSil(id);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }
    }
}
