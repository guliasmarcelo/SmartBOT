using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Collections.Generic;



namespace SmartBOT;

public class OpenAIService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    // Histórico de mensagens
    private readonly List<object> _messages;

    public OpenAIService(string apiKey)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.openai.com/v1/")
        };

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        // Set toCamelCase, expected format from OpenAI.
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        // Inicializa o histórico com uma mensagem do sistema
        _messages = new List<object>
        {
            new 
            { 
                role = "system", 
                content = "You are a support assistant whose goal is to answer questions about Tela and its products, and your name is ClaudIA." 
            }
        };
    }

    public async Task<string> SendChatMessageAsync(string model, string userMessage)
    {
        // Adiciona a mensagem do usuário ao histórico
        _messages.Add(new { role = "user", content = userMessage });

        // Cria o corpo da requisição com o histórico completo
        var requestBody = new
        {
            model,
            messages = _messages
        };

        // Envia a requisição
        var content = new StringContent(JsonSerializer.Serialize(requestBody, _jsonOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("chat/completions", content);

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

        // Adiciona a resposta do assistente ao histórico
        _messages.Add(new { role = "assistant", content = assistantMessage });

        return assistantMessage ?? throw new Exception("Error when trying to answear the question!");
    }

}

