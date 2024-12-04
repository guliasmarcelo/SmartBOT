using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;


namespace SmartBOT.WebAPI.Core;

public class OpenAIEmbeddingsService : IEmbeddingsService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string? _apiKey;

    public OpenAIEmbeddingsService(IConfiguration configuration)
    {
        _apiKey = configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(_apiKey))
        {
            throw new Exception("OpenAI API Key not found in configuration.");
        }

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.openai.com/v1/")
        };

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);

        // Configuração para JSON em formato camelCase, esperado pela OpenAI
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<float[]> GetEmbeddingAsync(string inputText)
    {
        // Cria o corpo da requisição para o endpoint /v1/embeddings
        var requestBody = new
        {
            model = "text-embedding-3-large",
            input = inputText
        };

        // Serializa o corpo da requisição
        var content = new StringContent(JsonSerializer.Serialize(requestBody, _jsonOptions), Encoding.UTF8, "application/json");

        // Envia a requisição para o endpoint
        var response = await _httpClient.PostAsync("embeddings", content);

        // Verifica se houve erro na resposta
        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro na API de embeddings: {error}");
        }

        // Processa a resposta
        var responseBody = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(responseBody);

        // Extrai os valores do embedding
        var embedding = jsonDocument.RootElement
            .GetProperty("data")[0]
            .GetProperty("embedding")
            .EnumerateArray()
            .Select(x => x.GetSingle())
            .ToArray();

        return embedding;
    }



}

