using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Services;

namespace MusicApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ArtistController : ControllerBase
    {
        private readonly ArtistService _artistService;

        public ArtistController(ArtistService artistService)
        {
            _artistService = artistService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var artists = await _artistService.GetAllAsync();
            return Ok(artists);
        }
    }
}
