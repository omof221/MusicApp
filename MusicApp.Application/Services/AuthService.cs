using MusicApp.Infrastructure;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using MusicApp.Domain.DTOs;
using MusicApp.Domain.Entities;

using MusicApp.Application.Abstractions;

namespace MusicApp.Application.Services;

public class AuthService : IAuthService
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _cfg;

    public AuthService(AppDbContext db, IConfiguration cfg)
    {
        _db = db;
        _cfg = cfg;
    }

    public async Task RegisterAsync(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("Bu e-posta zaten kayıtlı.");

        var user = new User
        {
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            MembershipLevel = dto.MembershipLevel
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();
    }

    public async Task<string> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("E-posta veya şifre hatalı.");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim("membership_level", user.MembershipLevel.ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_cfg["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            _cfg["Jwt:Issuer"],
            _cfg["Jwt:Audience"],
            claims,
            expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_cfg["Jwt:ExpireMinutes"])),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
