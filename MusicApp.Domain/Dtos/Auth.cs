namespace MusicApp.Domain.DTOs;
public record RegisterDto(string Email, string Password, int MembershipLevel);
public record LoginDto(string Email, string Password);
public record SongDto(int Id, string Title, string Artist, int Level, string Genre, int DurationSec, string CoverUrl);
public record RecommendationItemDto(int SongId, string Title, string Artist, int Level, string CoverUrl, float Score);
public record RecommendationResponse(List<RecommendationItemDto> Items, string? Explain);
public record LatestSongDto(
    int Id,
    string Title,
    string Artist,
    string Genre,
    string CoverUrl,
    DateTime CreatedAt
);
public record ArtistDto(
    int Id,
    string FullName,
    string Country,
    string Genre,
    string ImageUrl
);