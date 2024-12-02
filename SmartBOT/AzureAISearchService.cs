using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SmartBOT
{
    /// <summary>
    /// Classe responsável por trazer a base de conhecimento através de uma busca vetorial
    /// </summary>
    public class AzureAISearchService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly string _azureApiKey;

        public AzureAISearchService()
        {            
            _azureApiKey = "RG3f9GfswPkkxYKKPRX3wqObtBpdvlwQ9etiv1rX7TAzSeC677ln";

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://claudia-db.search.windows.net/")
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
}
