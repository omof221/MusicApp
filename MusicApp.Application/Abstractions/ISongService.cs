using MusicApp.Domain.DTOs;

namespace MusicApp.Application.Abstractions;

public interface ISongService
{
    Task<List<SongDto>> GetAllAsync();
    Task<List<SongDto>> GetSongsByLevelAsync(int level);
    Task PlayAsync(int userId, int songId, int userLevel);
    Task RecordUserPlayAsync(int userId, int songId);
}
