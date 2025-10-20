using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MusicApp.Application.Abstractions;
using MusicApp.Application.Services;
using MusicApp.Infrastructure;
using System.Security.Claims;

namespace MusicApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SongsController : ControllerBase
    {
        private readonly ISongService _songService;
        private readonly AppDbContext _db;

        public SongsController(ISongService songService, AppDbContext db)
        {
            _songService = songService;
            _db = db;
        }

        [HttpGet("latest")]
        public async Task<IActionResult> GetLatestSongs([FromQuery] int count = 10)
        {
            var songs = await _db.Songs
                .OrderByDescending(s => s.CreatedAt)
                .Take(count)
                .Select(s => new MusicApp.Domain.DTOs.LatestSongDto(
                    s.Id,
                    s.Title,
                    s.Artist,
                    s.Genre,
                    s.CoverUrl,
                    s.CreatedAt
                ))
                .ToListAsync();

            return Ok(songs);
        }


        // 🎧 1️⃣ Kullanıcının seviyesine uygun şarkıları getir
        [Authorize]
        [HttpGet("by-level")]
        public async Task<IActionResult> GetSongsByLevel()
        {
            var level = int.Parse(User.FindFirstValue("membership_level")!);
            var songs = await _songService.GetSongsByLevelAsync(level);
            return Ok(songs);
        }

        // 🎵 2️⃣ Kullanıcının bir şarkıyı dinlediğini kaydet
        [Authorize]
        [HttpPost("play/{songId}")]
        public async Task<IActionResult> PlaySong(int songId)
        {
            Console.WriteLine("🎯 Kullanıcı kimlik bilgileri:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }

            var userId = int.Parse(User.FindFirstValue("sub")!);
            Console.WriteLine("🎯 Token içeriği:");
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"{claim.Type} = {claim.Value}");
            }
            var userLevel = int.Parse(User.FindFirstValue("membership_level")!);

            await _songService.PlayAsync(userId, songId, userLevel);

            return Ok(new { message = "Dinlenme geçmişine eklendi." });
        }
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var songs = await _songService.GetAllAsync();
            return Ok(songs);
        }

    }
}
