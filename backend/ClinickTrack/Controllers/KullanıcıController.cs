using ClinickCore.DTOs;
using ClinickService.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClinickTrackApi.Controllers
{
    [Route("api/Kullanici")]
    [ApiController]
    [Authorize]
    public class KullanıcıController : ControllerBase
    {
        private readonly IKullanıcıService _kullanıcıService;

        public KullanıcıController(IKullanıcıService kullanıcıService)
        {
            _kullanıcıService = kullanıcıService;
        }


        [AllowAnonymous]
        [HttpPost("giris")]
        public IActionResult Login([FromBody] KullanıcıGirisDto dto)
        {
            var sonuc = _kullanıcıService.Login(dto);
            if (!sonuc.IsSuccess)
            {
                return Unauthorized(sonuc);
            }
            return Ok(sonuc);
        }

        [HttpPost("ilkParolaBelirle")]
        public IActionResult İlkParolaBelirle([FromBody] İlkParolaBelirleDto dto)
        {
            var sonuc = _kullanıcıService.İlkParolaBelirle(dto);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }


        [AllowAnonymous]
        [HttpPost("kayitOl")]
        public IActionResult HastaKayıt([FromBody] KullanıcıKayıtDto dto)
        {
            var sonuc = _kullanıcıService.HastaKayıt(dto);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }


        [Authorize(Roles = "Admin")]
        [HttpPost("createUser")]
        public IActionResult KullanıcıOlustur([FromBody] KullanıcıOlusturDto dto)
        {
            var sonuc = _kullanıcıService.KullanıcıOlustur(dto);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("getAll")]
        public IActionResult TumKullanıcılarıGetir()
        {
            var sonuc = _kullanıcıService.TumKullanıcılarıGetir();
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }


        [Authorize(Roles = "Admin, Doktor")]
        [HttpGet("getById/{id}")]
        public IActionResult KullanıcıGetirById(int id)
        {
            var sonuc = _kullanıcıService.KullanıcıGetirById(id);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }


        [Authorize(Roles = "Admin")]
        [HttpGet("getByEmail")]
        public IActionResult KullanıcıGetirByEmail(string email)
        {
            var sonuc = _kullanıcıService.KullanıcıGetirByEmail(email);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }


        [Authorize(Roles = "Admin")]
        [HttpPut("update/{id}")]
        public IActionResult KullanıcıGuncelle(int id, [FromBody] KullanıcıOlusturDto dto)
        {
            var sonuc = _kullanıcıService.KullanıcıGuncelle(id, dto);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [Authorize]
        [HttpPut("updateEmail/{id}")]
        public IActionResult EmailGuncelle(int id, [FromBody] string yeniEmail)
        {
            var sonuc = _kullanıcıService.EmailGuncelle(id, yeniEmail);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }

        [Authorize(Roles = "Admin,Doktor,Hasta")]
        [HttpPut("updatePhone/{id}")]
        public IActionResult TelefonGuncelle(int id, [FromBody] string yeniTelefon)
        {
            var sonuc = _kullanıcıService.TelefonGuncelle(id, yeniTelefon);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }


        [Authorize]
        [HttpPut("updatePassword/{id}")]
        public IActionResult ParolaGuncelle(int id, [FromBody] ParolaGuncelleDto dto)
        {
            var sonuc = _kullanıcıService.ParolaGuncelle(id, dto.EskiParola, dto.YeniParola);
            if (!sonuc.IsSuccess)
            {
                return BadRequest(sonuc);
            }
            return Ok(sonuc);
        }


        [Authorize(Roles = "Admin")]
        [HttpDelete("delete/{id}")]
        public IActionResult KullanıcıSil(int id)
        {
            var sonuc = _kullanıcıService.KullanıcıSil(id);
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }

        [Authorize]
        [HttpGet("profil")]
        public IActionResult ProfileGit()
        {
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized("Kullanıcı bilgisi bulunamadı.");
            }

            var sonuc = _kullanıcıService.KullanıcıGetirById(int.Parse(currentUserId));
            if (!sonuc.IsSuccess)
            {
                return NotFound(sonuc);
            }
            return Ok(sonuc);
        }
    }
}