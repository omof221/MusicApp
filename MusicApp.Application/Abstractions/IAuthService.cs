using MusicApp.Domain.DTOs;

namespace MusicApp.Application.Abstractions;

public interface IAuthService
{
    Task RegisterAsync(RegisterDto dto);
    Task<string> LoginAsync(LoginDto dto); // JWT token döner
}
