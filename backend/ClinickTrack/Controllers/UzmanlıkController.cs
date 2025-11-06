using ClinickCore.DTOs;
using ClinickService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinickTrackApi.Controllers
{
    [Route("api/Uzmanlik")]
    [ApiController]
    public class UzmanlıkController : ControllerBase
    {
        private readonly IUzmanlıkService _uzmanlıkService;
        public UzmanlıkController(IUzmanlıkService uzmanlıkService)
        {
            _uzmanlıkService = uzmanlıkService;
        }

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
