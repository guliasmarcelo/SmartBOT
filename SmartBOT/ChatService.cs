using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

public class ChatService
{
    private readonly HttpClient _httpClient;
    private const string ApiUrl = "https://api.openai.com/v1/chat/completions";
    private readonly string _apiKey;

    // Lista para armazenar o histórico de mensagens
    private readonly List<object> _messages;

    public ChatService(string apiKey)
    {
        _apiKey = apiKey;
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {_apiKey}");

        // Inicializa o histórico com uma mensagem do sistema
        _messages = new List<object>
        {
            new { role = "system", content = "Você é um assistente útil." }
        };
    }

    public async Task<string> GetResponseAsync(string userMessage)
    {
        // Adiciona a mensagem do usuário ao histórico
        _messages.Add(new { role = "user", content = userMessage });

        // Cria o corpo da requisição incluindo o histórico completo
        var requestBody = new
        {
            model = "gpt-4o",
            messages = _messages
        };

        var response = await _httpClient.PostAsJsonAsync(ApiUrl, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro na API: {error}");
        }

        var result = await response.Content.ReadFromJsonAsync<JsonDocument>();
        var content = result.RootElement
                            .GetProperty("choices")[0]
                            .GetProperty("message")
                            .GetProperty("content")
                            .GetString();

        // Adiciona a resposta da OpenAI ao histórico
        _messages.Add(new { role = "assistant", content });

        return content ?? "Desculpe, não consegui entender.";
    }
}
