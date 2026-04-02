using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace VoiceInput.LLM
{
    public class LLMRefiner
    {
        private readonly string _apiBaseUrl;
        private readonly string _apiKey;

        public LLMRefiner(string apiBaseUrl, string apiKey)
        {
            _apiBaseUrl = apiBaseUrl;
            _apiKey = apiKey;
        }

        public async Task<string> RefineAsync(string input)
        {
            var requestBody = JsonSerializer.Serialize(new
            {
                model = "gpt-4",
                messages = new[]
                {
                    new {role = "system", content = "Correct obvious transcription errors without making any unnecessary changes."},
                    new {role = "user", content = input}
                }
            });

            using var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");
            var response = await httpClient.PostAsync(_apiBaseUrl, 
                new StringContent(requestBody, Encoding.UTF8, "application/json"));
            
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();

            var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
            return jsonResponse.GetProperty("choices")[0].GetProperty("message").GetProperty("content").GetString();
        }
    }
}