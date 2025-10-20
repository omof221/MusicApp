using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Services;

namespace MusicApp.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : ControllerBase
    {
        private readonly RecommendationService _recommendationService;
        private readonly ListeningAnalysisService _analysisService;

        public RecommendationController(
            RecommendationService recommendationService,
            ListeningAnalysisService analysisService)
        {
            _recommendationService = recommendationService;
            _analysisService = analysisService;
        }

        // 🎧 Kullanıcıya özel şarkı önerileri (genel öneriler)
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetRecommendations([FromQuery] int top = 10)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized("Kullanıcı kimliği çözümlenemedi.");

                var userLevel = GetUserLevel();

                var recommendations = await _recommendationService.RecommendForUserAsync(userId.Value, userLevel, top);

                if (recommendations == null || recommendations.Items.Count == 0)
                    return Ok(new { message = "Henüz önerilecek şarkı bulunamadı." });

                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Beklenmedik hata oluştu.", details = ex.Message });
            }
        }

        // 🧠 Kullanıcının dinleme alışkanlıklarını analiz et
        [Authorize]
        [HttpGet("analysis")]
        public async Task<IActionResult> AnalyzeListeningHabits()
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized("Kullanıcı kimliği çözümlenemedi.");

                var analysis = await _analysisService.AnalyzeUserHabitsAsync(userId.Value);

                if (string.IsNullOrWhiteSpace(analysis))
                    return Ok(new { message = "Henüz analiz yapılacak veri bulunamadı." });

                return Ok(new { message = analysis });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Analiz işlemi sırasında hata oluştu.", details = ex.Message });
            }
        }

        // 🎯 Kullanıcının en çok dinlediği türlere göre 4 öneri
        [Authorize]
        [HttpGet("by-genre")]
        public async Task<IActionResult> RecommendByGenre([FromQuery] int top = 4)
        {
            try
            {
                var userId = GetUserId();
                if (userId == null)
                    return Unauthorized("Kullanıcı kimliği çözümlenemedi.");

                var userLevel = GetUserLevel();

                var response = await _recommendationService.RecommendByFavouriteGenresAsync(userId.Value, userLevel, top);

                if (response == null || response.Items.Count == 0)
                    return Ok(new { message = "Bu türlerde önerilecek şarkı bulunamadı." });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Tür bazlı öneri sırasında hata oluştu.", details = ex.Message });
            }
        }

        #region 🔒 Helper Methods

        private int? GetUserId()
        {
            var userIdClaim = User.FindFirstValue("sub")
                              ?? User.FindFirstValue("id")
                              ?? User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userIdClaim))
                return null;

            return int.TryParse(userIdClaim, out var id) ? id : null;
        }

        private int GetUserLevel()
        {
            var userLevelClaim = User.FindFirstValue("membership_level");
            return string.IsNullOrEmpty(userLevelClaim) ? 1 : int.Parse(userLevelClaim);
        }

        #endregion
    }
}
