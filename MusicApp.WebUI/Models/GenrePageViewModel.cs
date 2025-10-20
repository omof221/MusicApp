namespace MusicApp.WebUI.Models
{
    public class GenrePageViewModel
    {
        public List<MusicApp.Domain.Entities.Song> LevelSongs { get; set; } = new();
        public List<MusicApp.Domain.DTOs.RecommendationItemDto> AiRecommendations { get; set; } = new();
        public string? AiMessage { get; set; }
    }
}
