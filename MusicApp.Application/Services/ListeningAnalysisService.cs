using Microsoft.EntityFrameworkCore;
using MusicApp.Infrastructure;
using MusicApp.Infrastructure.ML;
using MusicApp.Infrastructure.OpenAi;

namespace MusicApp.Application.Services
{
    public class ListeningAnalysisService
    {
        private readonly AppDbContext _db;
        private readonly RecommendationModel _model;
        private readonly OpenAiService _ai;

        public ListeningAnalysisService(AppDbContext db, RecommendationModel model, OpenAiService ai)
        {
            _db = db;
            _model = model;
            _ai = ai;
        }

        /// <summary>
        /// Kullanıcının son 10 dinleme alışkanlığına göre AI yorumu üretir.
        /// </summary>
        public async Task<string> AnalyzeUserHabitsAsync(int userId)
        {
            // 1️⃣ Kullanıcının son 10 dinleme kaydını çek
            var lastPlays = await _db.UserPlays
                .Include(p => p.Song)
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.PlayedAt)
                .Take(10)
                .ToListAsync();

            if (!lastPlays.Any())
                return "Henüz dinleme geçmişiniz bulunmuyor. Lütfen birkaç şarkı dinleyin 🎵";

            // 2️⃣ Genre bazında sayım yap (örnek: Pop=4, Rock=3, Jazz=2)
            var genreCount = lastPlays
                .GroupBy(p => p.Song.Genre)
                .ToDictionary(g => g.Key, g => g.Count());

            // 3️⃣ En çok dinlenen türü belirle
            var topGenre = genreCount.OrderByDescending(x => x.Value).First().Key;
            var total = lastPlays.Count;
            var dominantRatio = (double)genreCount[topGenre] / total;

            // 4️⃣ ML.NET modeliyle temel skor çıkar (örnek: 0.7 -> güçlü tercih)
            var mlScore = _model.PredictScore(userId, genreCount.Count);

            // 5️⃣ OpenAI’ye analiz için gönderilecek prompt
            var prompt = $@"
Kullanıcının müzik dinleme verileri:
- En çok dinlenen tür: {topGenre}
- Bu türün oranı: %{dominantRatio * 100:0}
- ML.NET analiz skoru: {mlScore:0.00}

Bu verileri değerlendirip kullanıcının ruh hali hakkında kısa, doğal bir Türkçe yorum üret.
Metin 2 cümleyi geçmesin ve samimi dille yaz.
";

            // 6️⃣ OpenAI servisini kullan
            var aiResponse = await _ai.ExplainAsync("Kullanıcı dinleme analizi", new List<string> { prompt });

            return aiResponse;
        }
    }
}
