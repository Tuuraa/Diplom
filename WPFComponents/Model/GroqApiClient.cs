using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace WPFComponents.Model
{
    public class GroqApiClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public GroqApiClient(string baseUrl, string apiKey)
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl.TrimEnd('/');
            _apiKey = apiKey;
        }

        public async Task<string> SendQueryAsync(string inputMessage, string model, double temperature = 1.0, int maxTokens = 1024, double topP = 1.0, bool stream = false, string responseFormat = "json_object", string stop = null)
        {
            var url = $"https://api.groq.com/openai/v1/chat/completions";
            var payload = new
            {
                messages = new[]
                {
            new { role = "system", content = "Provide the response in JSON format." },
            new { role = "user", content = inputMessage }
        },
                model,
                temperature,
                max_tokens = maxTokens,
                top_p = topP,
                stream,
                response_format = new { type = responseFormat },
                stop
            };

            var requestContent = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = requestContent
            };

            request.Headers.Add("Authorization", $"Bearer {_apiKey}");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                throw new Exception($"Error from Groq API: {response.StatusCode} - {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            // Извлечение content из choices[0].message.content
            using var jsonDocument = JsonDocument.Parse(responseContent);
            var root = jsonDocument.RootElement;
            var content = root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return content;

        }


    }
}