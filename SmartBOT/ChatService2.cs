using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SmartBOT
{
    public class ChatService2
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
        private readonly string _apiKey;
        private readonly JsonSerializerOptions _jsonOptions;

        public ChatService2(string apiKey)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
        }

        public async Task<string> GetResponseAsync(string userMessage)
        {
            // Configura o corpo da requisição
            var requestBody = new
            {
                model = "gpt-4o",
                messages = new[]
                {
                new { role = "system", content = "Você é um assistente útil." },
                new { role = "user", content = userMessage }
            }
            };

            // Serializa o corpo para JSON
            var content = new StringContent(JsonSerializer.Serialize(requestBody, _jsonOptions), Encoding.UTF8, "application/json");

            // Envia a requisição para a API da OpenAI
            var response = await _httpClient.PostAsync(ApiUrl, content);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                throw new Exception($"Erro na API: {error}");
            }

            // Processa a resposta
            var responseBody = await response.Content.ReadAsStringAsync();
            using var jsonDocument = JsonDocument.Parse(responseBody);
            var assistantMessage = jsonDocument.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return assistantMessage ?? "Erro ao processar a resposta.";
        }

    }
}
