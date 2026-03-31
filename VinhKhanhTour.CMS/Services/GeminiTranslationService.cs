using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace VinhKhanhTour.CMS.Services
{
    public class GeminiTranslationService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<GeminiTranslationService> _logger;

        public GeminiTranslationService(IConfiguration config, ILogger<GeminiTranslationService> logger)
        {
            _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(30) };
            _apiKey = config["Gemini:ApiKey"] ?? "";
            _logger = logger;
        }

        public bool IsConfigured => !string.IsNullOrWhiteSpace(_apiKey);

        public class PoiTranslationResult
        {
            public Dictionary<string, string> Name { get; set; } = new();
            public Dictionary<string, string> Description { get; set; } = new();
            public Dictionary<string, string> TtsScript { get; set; } = new();
        }

        /// <summary>
        /// Translates Name, Description, and TtsScript in a single API call to save quotas and avoid rate limits.
        /// </summary>
        public async Task<PoiTranslationResult> TranslatePoiAsync(string name, string description, string ttsScript)
        {
            var result = new PoiTranslationResult();
            if (!IsConfigured) throw new InvalidOperationException("Gemini API Key is not configured.");

            var prompt = "Translate the following information about a restaurant/POI in Vietnam into these 10 languages: en, es, fr, de, zh, ja, ko, ru, it, pt.\n" +
                "Keep the translation natural and concise. If the Name contains a street/restaurant name, retain the Vietnamese proper noun part.\n\n" +
                $"Name: \"{name}\"\n" +
                $"Description: \"{description}\"\n" +
                $"TTS Script: \"{ttsScript}\"\n\n" +
                "Return ONLY a valid JSON object with the exact structure below (no markdown, no explanation):\n" +
                "{\n" +
                "  \"Name\": {\n" +
                "    \"en\": \"...\", \"es\": \"...\", \"fr\": \"...\", \"de\": \"...\", \"zh\": \"...\", \"ja\": \"...\", \"ko\": \"...\", \"ru\": \"...\", \"it\": \"...\", \"pt\": \"...\"\n" +
                "  },\n" +
                "  \"Description\": {\n" +
                "    \"en\": \"...\", \"es\": \"...\", \"fr\": \"...\", \"de\": \"...\", \"zh\": \"...\", \"ja\": \"...\", \"ko\": \"...\", \"ru\": \"...\", \"it\": \"...\", \"pt\": \"...\"\n" +
                "  },\n" +
                "  \"TtsScript\": {\n" +
                "    \"en\": \"...\", \"es\": \"...\", \"fr\": \"...\", \"de\": \"...\", \"zh\": \"...\", \"ja\": \"...\", \"ko\": \"...\", \"ru\": \"...\", \"it\": \"...\", \"pt\": \"...\"\n" +
                "  }\n" +
                "}";

            var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";

            var requestBody = new
            {
                contents = new[]
                {
                    new { parts = new[] { new { text = prompt } } }
                },
                generationConfig = new
                {
                    temperature = 0.3,
                    response_mime_type = "application/json" // Force JSON output mode for Gemini 2.0
                }
            };

            var response = await _httpClient.PostAsJsonAsync(url, requestBody);
            var responseText = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Gemini API Error ({response.StatusCode}): {responseText}");
            }

            // Parse Gemini response
            using var doc = JsonDocument.Parse(responseText);
            var content = doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString();

            if (string.IsNullOrWhiteSpace(content)) return result;

            // Clean up markdown code fences if present
            content = content.Trim();
            if (content.StartsWith("```json")) content = content[7..];
            if (content.StartsWith("```")) content = content[3..];
            if (content.EndsWith("```")) content = content[..^3];
            content = content.Trim();

            var parsed = JsonSerializer.Deserialize<PoiTranslationResult>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            return parsed ?? result;
        }
    }
}
