using Microsoft.AspNetCore.Mvc;
using MusicApp.Application.Abstractions;
using MusicApp.Domain.DTOs;

namespace MusicApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _svc;
    public AuthController(IAuthService svc)
    {
        _svc = svc;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto)
    {
        await _svc.RegisterAsync(dto);
        return Ok("Kayıt başarılı.");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto dto)
    {
        var token = await _svc.LoginAsync(dto);
        return Ok(new { token });
    }
}
