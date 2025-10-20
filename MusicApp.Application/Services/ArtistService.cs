using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.DTOs;
using MusicApp.Domain.Entities;
using MusicApp.Infrastructure;

namespace MusicApp.Application.Services;

public class ArtistService
{
    private readonly AppDbContext _db;

    public ArtistService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<ArtistDto>> GetAllAsync()
    {
        return await _db.Artists
            .Select(a => new ArtistDto(
                a.Id,
                $"{a.FirstName} {a.LastName}",
                a.Country,
                a.Genre,
                a.ImageUrl
            ))
            .ToListAsync();
    }
}
