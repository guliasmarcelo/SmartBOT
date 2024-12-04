using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


namespace SmartBOT.WebAPI.Core;

/// <summary>
/// Classe responsável por trazer a base de conhecimento através de uma busca vetorial
/// </summary>
public class AzureAISearchService : IVectorDbSearchService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string? _azureApiKey;
    private readonly string? _azureBaseAddress;

    public AzureAISearchService(IConfiguration configuration)
    {
        _azureApiKey = configuration["AzureAISearch:ApiKey"];
        if (string.IsNullOrEmpty(_azureApiKey))
        {
            throw new Exception("Azure API Key not found in configuration.");
        }

        _azureBaseAddress = configuration["AzureAISearch:BaseAddress"];
        if (string.IsNullOrEmpty(_azureBaseAddress))
        {
            throw new Exception("Azure Base Address not found in configuration.");
        }

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_azureBaseAddress)
        };

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _azureApiKey);
        _httpClient.DefaultRequestHeaders.Add("api-key", _azureApiKey);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<List<SearchResult>> SearchAsync(float[] embeddings, string projectName, int top, int k)
    {
        var requestBody = new
        {
            count = true,
            select = "content, type",
            top = top,
            filter = $"projectName eq '{projectName}'",
            vectorQueries = new[]
            {
                new
                {
                    vector = embeddings,
                    k = k,
                    fields = "embeddings",
                    kind = "vector"
                }
            }
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody, _jsonOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("indexes/claudia-ids-index-large/docs/search?api-version=2023-11-01", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro na API de pesquisa vetorial: {error}");
        }

        var responseBody = await response.Content.ReadAsStringAsync();
        var searchResults = JsonSerializer.Deserialize<SearchResponse>(responseBody, _jsonOptions);

        return searchResults?.Value ?? new List<SearchResult>();
    }
}

public class SearchResponse
{
    public List<SearchResult> Value { get; set; }
}

public class SearchResult
{
    [JsonPropertyName("content")]
    public string Content { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("@search.score")]
    public float Score { get; set; }
}