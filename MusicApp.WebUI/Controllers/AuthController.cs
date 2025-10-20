using Microsoft.AspNetCore.Mvc;
using MusicApp.Domain.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

namespace MusicApp.WebUI.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterDto dto)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7273/");

            try
            {
                var response = await client.PostAsJsonAsync("api/Auth/register", dto);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Kayıt başarılı! Giriş yapabilirsiniz.";
                    return RedirectToAction("Login", "Auth");
                }

                // API hata mesajını yakala
                var error = await response.Content.ReadAsStringAsync();
                ViewBag.Error = $"Kayıt başarısız: {error}";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Bir hata oluştu: {ex.Message}";
                return View();
            }
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7273/");

            var response = await client.PostAsJsonAsync("api/Auth/login", dto);
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Giriş başarısız.";
                return View();
            }

            var json = await response.Content.ReadFromJsonAsync<System.Text.Json.JsonElement>();
            string token = json.GetProperty("token").GetString() ?? "";

            HttpContext.Session.SetString("token", token);


            return RedirectToAction("Genres", "Home");
        }
    }
}
