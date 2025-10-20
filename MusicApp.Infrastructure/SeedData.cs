using MusicApp.Domain.Entities;

namespace MusicApp.Infrastructure;

public static class SeedData
{
    public static async Task EnsureSeedAsync(AppDbContext db)
    {
        if (!db.Songs.Any())
        {
            db.Songs.AddRange(
                new Song { Title = "Skyline", Artist = "Nova", Level = 1, Genre = "Pop", DurationSec = 210 },
                new Song { Title = "Riff Storm", Artist = "Volt", Level = 2, Genre = "Rock", DurationSec = 240 },
                new Song { Title = "Deep Focus", Artist = "AmbientX", Level = 3, Genre = "Ambient", DurationSec = 180 },
                new Song { Title = "Low End", Artist = "Bassman", Level = 4, Genre = "EDM", DurationSec = 200 },
                new Song { Title = "Prestige", Artist = "Orchestra", Level = 5, Genre = "Classical", DurationSec = 300 }
            );
            await db.SaveChangesAsync();
        }
    }
}
