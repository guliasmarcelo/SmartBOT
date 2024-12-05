using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Configuration;

namespace SmartBOT.WebAPI.Core;

/// <summary>
/// Classe stateless para realizar o chat com o agente de HelpDesk da Tesla.
/// Gerencia mensagens e histórico por meio de argumentos.
/// </summary>
public class OpenAIChatService : IChatService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly IChatHistoryRepository _chatHistoryRepository;
    private readonly string? _OpenAIApiKey;
    private readonly string? _OpenAIBaseAddress;

    public OpenAIChatService(IConfiguration configuration, IChatHistoryRepository chatHistoryRepository)
    {
        _chatHistoryRepository = chatHistoryRepository;

        _OpenAIApiKey = configuration["OpenAI:ApiKey"];
        if (string.IsNullOrEmpty(_OpenAIApiKey))
        {
            throw new Exception("OpenAI API Key not found in configuration.");
        }

        _OpenAIBaseAddress = configuration["OpenAI:BaseAddress"];
        if (string.IsNullOrEmpty(_OpenAIBaseAddress))
        {
            throw new Exception("OpenAI BaseAddress not found in configuration.");
        }


        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(_OpenAIBaseAddress)
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _OpenAIApiKey);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };
    }

    public async Task<List<ChatMessage>> LoadChatHistoryAsync(string helpdeskId)
    {
        return await _chatHistoryRepository.LoadChatHistoryAsync(helpdeskId);
    }

    public async Task<string> SendUserMessageAsync(
     string helpdeskId,
     List<ChatMessage> messages,
     string userMessage,
     string knowledgeBase,
     string model,
     string instructionsSystemMessage)
    {
        // Verificar e adicionar a mensagem de sistema com isntruçoes  apenas uma vez
        if (!messages.Any(m => m.Role == "system" && m.Content == instructionsSystemMessage))
        {
            var systemMessageObj = new ChatMessage
            {
                Role = "system",
                Content = instructionsSystemMessage
            };
            messages.Insert(0, systemMessageObj); // Garante que esteja no início
            await _chatHistoryRepository.SaveMessageAsync(helpdeskId, systemMessageObj);
        }

        // Adicionar FAQ como mensagem do sistema, se fornecida
        if (!string.IsNullOrWhiteSpace(knowledgeBase))
        {
            var knowledgeMessage = new ChatMessage
            {
                Role = "system",
                Content = $"{knowledgeBase}"
            };

            // Adicionar após a mensagem de sistema principal, mas antes da mensagem do usuário
            if (!messages.Any(m => m.Role == "system" && m.Content.StartsWith("Here is some FAQ information")))
            {
                messages.Insert(1, knowledgeMessage); // Após a mensagem de sistema principal
                await _chatHistoryRepository.SaveMessageAsync(helpdeskId, knowledgeMessage);
            }
        }

        // Adicionar mensagem do usuário
        var userMessageObj = new ChatMessage { Role = "user", Content = userMessage };
        messages.Add(userMessageObj); // Sempre no final
        await _chatHistoryRepository.SaveMessageAsync(helpdeskId, userMessageObj);

        // Preparar requisição para OpenAI
        var requestBody = new
        {
            model = model,
            messages = messages
        };

        var content = new StringContent(JsonSerializer.Serialize(requestBody, _jsonOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro na API: {error}");
        }

        // Processar resposta
        var responseBody = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(responseBody);
        var assistantMessage = jsonDocument.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        // Salvar resposta do assistente
        if (assistantMessage != null)
        {
            var assistantMessageObj = new ChatMessage { Role = "assistant", Content = assistantMessage };
            messages.Add(assistantMessageObj);
            await _chatHistoryRepository.SaveMessageAsync(helpdeskId, assistantMessageObj);
        }

        return assistantMessage ?? throw new Exception("Error when trying to answer the question!");
    }

}

