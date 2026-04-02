using VoiceInput.App.Models;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace VoiceInput.App.LLM
{
    public class LlmRefiner
    {
        private readonly HttpClient _httpClient;
        private readonly AppSettings _settings;

        private const string SystemPrompt = @"You are a speech recognition text refiner. Your task is to correct obvious errors in transcribed speech text, focusing on:

1. Homophone corrections for Chinese (e.g., '配森' → 'Python', '杰森' → 'JSON')
2. Technical terms that were misheard (e.g., '艾皮艾' → 'API', '歇斯渴' → 'XSS')
3. Common English misspellings in mixed language text

IMPORTANT RULES:
- Only fix obvious errors. If the text looks correct, return it unchanged.
- Do not rephrase, rewrite, or add any content.
- Do not remove any words or punctuation.
- Preserve all original formatting and structure.
- If unsure, return the original text.

Examples:
Input: '我喜欢配森编程'
Output: '我喜欢Python编程'

Input: '使用杰森格式'
Output: '使用JSON格式'

Input: '这是一个正确的句子'
Output: '这是一个正确的句子'";

        public LlmRefiner(AppSettings settings)
        {
            _settings = settings;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_settings.ApiKey}");
        }

        public async Task<string> RefineTextAsync(string text)
        {
            if (string.IsNullOrEmpty(_settings.ApiKey)) return text;

            var request = new
            {
                model = _settings.LlmModel,
                messages = new[]
                {
                    new { role = "system", content = SystemPrompt },
                    new { role = "user", content = text }
                },
                stream = false
            };

            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_settings.ApiBaseUrl}/chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<JsonElement>(responseJson);
                var refinedText = result.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();

                return refinedText ?? text;
            }
            catch
            {
                return text;
            }
        }
    }
}