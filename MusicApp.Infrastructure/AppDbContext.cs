using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.Entities;
using MusicApp.Infrastructure;

namespace MusicApp.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Song> Songs => Set<Song>();
    public DbSet<UserPlay> UserPlays => Set<UserPlay>();
    public DbSet<Artist> Artists { get; set; }

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<User>().HasIndex(x => x.Email).IsUnique();
        b.Entity<UserPlay>()
            .HasOne(p => p.User).WithMany(u => u.Plays).HasForeignKey(p => p.UserId);
        b.Entity<UserPlay>()
            .HasOne(p => p.Song).WithMany(s => s.Plays).HasForeignKey(p => p.SongId);
    }
}
