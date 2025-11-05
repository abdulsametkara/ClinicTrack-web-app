using ClinickService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinickTrackApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoktorController : ControllerBase
    {
        private readonly IDoktorService _doktorService;
        public DoktorController(IDoktorService doktorService)
        {
            _doktorService = doktorService;
        }

        [HttpGet("TumDoktorlariGetir")]
        public IActionResult GetAll()
        {
            var doktorlar = _doktorService.TumDoktolarıGetir();
            return Ok(doktorlar);

        }
    }
}
