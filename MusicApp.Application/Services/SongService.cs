using Microsoft.EntityFrameworkCore;
using MusicApp.Application.Abstractions;
using MusicApp.Domain.DTOs;
using MusicApp.Domain.Entities;
using MusicApp.Infrastructure;

namespace MusicApp.Application.Services;

public class SongService : ISongService
{
    private readonly AppDbContext _db;

    public SongService(AppDbContext db)
    {
        _db = db;
    }

    // 🔹 Tüm şarkıları getir (Admin veya genel liste)
    public async Task<List<SongDto>> GetAllAsync()
    {
        return await _db.Songs
            .OrderBy(s => s.Level)
            .Select(s => new SongDto(s.Id, s.Title, s.Artist, s.Level, s.Genre, s.DurationSec,s.CoverUrl))
            .ToListAsync();
    }

    // 🔹 Kullanıcı seviyesine uygun şarkılar
    public async Task<List<SongDto>> GetSongsByLevelAsync(int level)
    {
        return await _db.Songs
            .Where(s => s.Level <= level)
            .OrderBy(s => s.Level)
            .Select(s => new SongDto(
                s.Id,
                s.Title,
                s.Artist,
                s.Level,
                s.Genre,
                s.DurationSec,
                s.CoverUrl
            ))
            .ToListAsync();
    }

    // 🔹 Kullanıcının şarkı dinleme kaydı oluşturma (Play)
    public async Task PlayAsync(int userId, int songId, int userLevel)
    {
        var song = await _db.Songs.FindAsync(songId) ?? throw new KeyNotFoundException("Şarkı bulunamadı.");

        if (userLevel < song.Level)
            throw new UnauthorizedAccessException("Üyelik seviyeniz bu şarkı için yetersiz.");

        _db.UserPlays.Add(new UserPlay
        {
            UserId = userId,
            SongId = songId,
            PlayedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }

    // 🔹 Basit dinleme kaydı (seviye kontrolsüz)
    public async Task RecordUserPlayAsync(int userId, int songId)
    {
        _db.UserPlays.Add(new UserPlay
        {
            UserId = userId,
            SongId = songId,
            PlayedAt = DateTime.UtcNow
        });

        await _db.SaveChangesAsync();
    }
}
