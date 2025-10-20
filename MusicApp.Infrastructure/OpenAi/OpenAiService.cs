using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace MusicApp.Infrastructure.OpenAi
{
    public class OpenAiService
    {
        private readonly HttpClient _http;
        private readonly string _model;

        public OpenAiService(IConfiguration cfg)
        {
            _http = new HttpClient { BaseAddress = new Uri("https://api.openai.com/v1/") };

            var apiKey = cfg["OpenAI:ApiKey"];
            _model = cfg["OpenAI:Model"] ?? "gpt-4o-mini";

            _http.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
        }

        /// <summary>
        /// Kullanıcıya önerilen şarkılar listesini Türkçe olarak açıklar.
        /// </summary>
        public async Task<string> ExplainAsync(string userProfile, List<string> songTitles)
        {
            var prompt = $@"
Kullanıcı profili: {userProfile}
Önerilen şarkılar: {string.Join(", ", songTitles)}
Bu listeyi kısa, samimi ve Türkçe bir dille açıkla.";

            var body = new
            {
                model = _model,
                messages = new[]
                {
                    new { role = "system", content = "Sen bir Türk müzik psikolojisi asistanısın. Kullanıcının müzik ruh halini çözümle." },
                    new { role = "user", content = prompt }
                },
                max_tokens = 250,
                temperature = 0.8
            };

            try
            {
                var response = await _http.PostAsJsonAsync("chat/completions", body);
                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return $"AI isteği başarısız oldu ({response.StatusCode}): {errorBody}";
                }

                var json = await response.Content.ReadFromJsonAsync<JsonElement>();

                // 🔹 Bazı modeller "choices -> message -> content" yapısını döner
                if (json.TryGetProperty("choices", out var choices))
                {
                    var first = choices[0];
                    if (first.TryGetProperty("message", out var msg) &&
                        msg.TryGetProperty("content", out var content))
                        return content.GetString() ?? "AI açıklaması alınamadı.";

                    // Eski modeller bazen "text" alanını döner
                    if (first.TryGetProperty("text", out var txt))
                        return txt.GetString() ?? "AI açıklaması alınamadı.";
                }

                return "AI yanıtı alınamadı veya beklenen formatta değil.";
            }
            catch (Exception ex)
            {
                return $"AI isteği başarısız oldu: {ex.Message}";
            }
        }
    }
}
