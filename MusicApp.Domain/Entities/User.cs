
namespace MusicApp.Domain.Entities;
public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = "";
    public string PasswordHash { get; set; } = "";
    public int MembershipLevel { get; set; } // 1..5
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public ICollection<UserPlay> Plays { get; set; } = new List<UserPlay>();
}


public class Song
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Artist { get; set; } = "";
    public int Level { get; set; } // 1..5 erişim seviyesi
    public string Genre { get; set; } = "";
    public int DurationSec { get; set; }
    public string? CoverUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<UserPlay> Plays { get; set; } = new List<UserPlay>();
}

// UserPlay.cs

public class UserPlay
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int SongId { get; set; }
    public DateTime PlayedAt { get; set; } = DateTime.UtcNow;

    public User User { get; set; } = null!;
    public Song Song { get; set; } = null!;
}

public class Artist
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Country { get; set; } = null!;
    public string ImageUrl { get; set; } = null!;
    public string Genre { get; set; } = null!; // Sanatçının yaptığı müzik türü
 }
