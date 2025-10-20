using Microsoft.AspNetCore.Mvc;
using MusicApp.Domain.DTOs;
using MusicApp.Domain.Entities;
using MusicApp.WebUI.Models;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text.Json;

namespace MusicApp.WebUI.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Artists()
        {
            var token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7273/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/artist");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Sanatçılar alınamadı.";
                return View(new List<ArtistDto>());
            }

            var artists = await response.Content.ReadFromJsonAsync<List<ArtistDto>>();
            return View(artists);
        }

        [HttpGet]
        public async Task<IActionResult> Genres()
        {
            var token = HttpContext.Session.GetString("token");

            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Error = "Token bulunamadı. Lütfen tekrar giriş yapın.";
                return RedirectToAction("Login", "Auth");
            }

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7273/");
            client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            Console.WriteLine($"🔑 TOKEN: {token.Substring(0, Math.Min(token.Length, 30))}...");

            var model = new MusicApp.WebUI.Models.GenrePageViewModel();

            // 🎶 1️⃣ Seviyeye göre şarkılar
            var responseLevel = await client.GetAsync("api/songs/by-level");
            if (responseLevel.IsSuccessStatusCode)
            {
                var songs = await responseLevel.Content.ReadFromJsonAsync<List<MusicApp.Domain.Entities.Song>>();
                model.LevelSongs = songs ?? new();
            }

            // 🤖 2️⃣ AI Tür bazlı öneriler
            var responseAi = await client.GetAsync("api/recommendation/by-genre?top=4");
            if (responseAi.IsSuccessStatusCode)
            {
                var aiData = await responseAi.Content.ReadFromJsonAsync<MusicApp.Domain.DTOs.RecommendationResponse>();
                if (aiData != null)
                {
                    model.AiRecommendations = aiData.Items ?? new();
                    model.AiMessage = aiData.Explain;
                }
            }

            // 🧠 3️⃣ AI Analiz çağrısı (dinleme geçmişine göre ruh hali)
            var analysisResponse = await client.GetAsync("api/recommendation/analysis");
            if (analysisResponse.IsSuccessStatusCode)
            {
                var json = await analysisResponse.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                if (json != null && json.ContainsKey("message"))
                    ViewBag.AiAnalysis = json["message"]?.ToString();
                else
                    ViewBag.AiAnalysis = "AI analizi alınamadı.";

            }
            else
            {
                ViewBag.AiAnalysis = "AI analizi alınamadı.";
            }
            Console.WriteLine(model);
            return View(model);
          
        }
        [HttpGet]
        public async Task<IActionResult> Discover()
        {
            var token = HttpContext.Session.GetString("token");
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7273/");
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // 🧠 1️⃣ AI Analizi
            string aiMessage = "Henüz analiz bulunamadı.";
            var analysisResponse = await client.GetAsync("api/recommendation/analysis");
            if (analysisResponse.IsSuccessStatusCode)
            {
                var json = await analysisResponse.Content.ReadFromJsonAsync<JsonElement>();
                if (json.TryGetProperty("message", out var msg))
                    aiMessage = msg.GetString() ?? aiMessage;
            }

            // 🤖 2️⃣ AI Tarafından Önerilen Şarkılar
            var recResponse = await client.GetAsync("api/recommendation/me?top=6");
            var recommendations = recResponse.IsSuccessStatusCode
                ? (await recResponse.Content.ReadFromJsonAsync<RecommendationResponse>())?.Items ?? new()
                : new();

            // 🎧 3️⃣ Kullanıcının erişebileceği tüm şarkılar
            var songsResponse = await client.GetAsync("api/songs/by-level");
            var allSongs = songsResponse.IsSuccessStatusCode
                ? (await songsResponse.Content.ReadFromJsonAsync<List<SongDto>>()) ?? new()
                : new();

            // 🆕 4️⃣ En son eklenen 10 şarkı
            var latestResponse = await client.GetAsync("api/songs/latest?count=10");
            var latestSongs = latestResponse.IsSuccessStatusCode
     ? (await latestResponse.Content.ReadFromJsonAsync<List<LatestSongDto>>()) ?? new()
     : new();


            // 🔙 ViewBag
            ViewBag.AIMessage = aiMessage;
            ViewBag.AllSongs = allSongs;
            ViewBag.LatestSongs = latestSongs;
            return View(recommendations);
        }
        public IActionResult MainPage()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> AllSongs()
        {
            var token = HttpContext.Session.GetString("token");

            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Login", "Auth");

            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("https://localhost:7273/");
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync("api/songs");

            List<SongDto> songs = new();
            if (response.IsSuccessStatusCode)
            {
                try
                {
                    songs = await response.Content.ReadFromJsonAsync<List<SongDto>>() ?? new();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"🎵 Şarkı listesi parse hatası: {ex.Message}");
                }
            }

            return View(songs);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
