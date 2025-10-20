using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.DTOs;
using MusicApp.Infrastructure;
using MusicApp.Infrastructure.ML;
using MusicApp.Infrastructure.OpenAi;
using System.Linq;

namespace MusicApp.Application.Services;

public class RecommendationService
{
    private readonly AppDbContext _db;
    private readonly RecommendationModel _model;
    private readonly OpenAiService _ai;

    public RecommendationService(AppDbContext db, RecommendationModel model, OpenAiService ai)
    {
        _db = db;
        _model = model;
        _ai = ai;
    }

    /// <summary>
    /// Kullanıcının geçmişine göre genel öneriler (10 şarkı)
    /// </summary>
    public async Task<RecommendationResponse> RecommendForUserAsync(int userId, int userLevel, int top = 10)
    {
        // Kullanıcının geçmişte dinlediği şarkı ID'leri
        var playedIds = await _db.UserPlays
            .Where(p => p.UserId == userId)
            .Select(p => p.SongId)
            .ToListAsync();

        // Kullanıcının seviyesine uygun, daha önce dinlemediği şarkılar
        var candidates = await _db.Songs
            .Where(s => s.Level <= userLevel && !playedIds.Contains(s.Id))
            .ToListAsync();

        // ML modeliyle skor hesapla
        var scored = candidates
            .Select(s => new RecommendationItemDto(
                s.Id,
    s.Title,
    s.Artist,
    s.Level,
    s.CoverUrl,                         // ✅ 5. parametre: string CoverUrl
    _model.PredictScore(userId, s.Id)
            ))
            .OrderByDescending(x => x.Score)
            .Take(top)
            .ToList();

        // Kullanıcı profili metni (AI için)
        var profile = $"Üyelik seviyesi: {userLevel}, geçmiş dinleme sayısı: {playedIds.Count}";

        // Eğer öneriler varsa, AI açıklaması oluştur
        var explain = scored.Any()
            ? await _ai.ExplainAsync(profile, scored.Select(i => i.Title).ToList())
            : "Yeni olduğun için erişilebilir popüler şarkıları öneriyorum.";

        return new RecommendationResponse(scored, explain);
    }

    /// <summary>
    /// 🎯 Kullanıcının en çok dinlediği türlere göre 4 şarkı önerir.
    /// </summary>
    public async Task<RecommendationResponse> RecommendByFavouriteGenresAsync(int userId, int userLevel, int top = 4)
    {
        // 1️⃣ Kullanıcının dinleme geçmişinden türleri çek
        var played = await _db.UserPlays
            .Include(p => p.Song)
            .Where(p => p.UserId == userId && p.Song != null)
            .Select(p => new { p.SongId, p.Song.Genre })
            .ToListAsync();

        if (!played.Any())
        {
            return new RecommendationResponse(new List<RecommendationItemDto>(),
                "Henüz yeterli dinleme geçmişin yok. Lütfen birkaç şarkı dinle 🎵");
        }

        var playedSongIds = played.Select(x => x.SongId).ToHashSet();

        // 2️⃣ En çok dinlenen türleri bul (ilk 3)
        var favGenres = played
            .Where(p => !string.IsNullOrEmpty(p.Genre))
            .GroupBy(p => p.Genre!)
            .OrderByDescending(g => g.Count())
            .Take(3)
            .Select(g => g.Key)
            .ToList();

        // 3️⃣ Kullanıcının seviyesine uygun, daha önce dinlemediği ve bu türlere ait şarkılar
        var candidates = await _db.Songs
            .Where(s => s.Level <= userLevel &&
                        !playedSongIds.Contains(s.Id) &&
                        favGenres.Contains(s.Genre!))
            .ToListAsync();

        if (!candidates.Any())
        {
            return new RecommendationResponse(new List<RecommendationItemDto>(),
                "Bu türlerde yeni şarkı önerisi bulunamadı 🎶");
        }

        // 4️⃣ ML modeliyle puanla ve sırala
        var scored = candidates
            .Select(s => new RecommendationItemDto(
                  s.Id,
    s.Title,
    s.Artist,
    s.Level,
    s.CoverUrl,                         // ✅ 5. parametre: string CoverUrl
    _model.PredictScore(userId, s.Id)
            ))
            .OrderByDescending(x => x.Score)
            .Take(top)
            .ToList();

        // 5️⃣ AI açıklaması oluştur
        var profile = $"Kullanıcının en çok dinlediği türler: {string.Join(", ", favGenres)}";
        var explain = await _ai.ExplainAsync(profile, scored.Select(s => s.Title).ToList());

        return new RecommendationResponse(scored, explain);
    }
}
