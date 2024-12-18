﻿using System.Net.Http.Headers;
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
     string instructionsMessage,
     string? clarificationMessage = null)
    {
        // Adicionar a mensagem de sistema na primeira interação
        if (!messages.Any())
        {
            var systemMessageObj = new ChatMessage
            {
                Role = "system",
                Content = instructionsMessage
            };
            messages.Add(systemMessageObj);
            await _chatHistoryRepository.SaveMessageAsync(helpdeskId, systemMessageObj);
        }


        // Adicionar FAQ como mensagem do sistema, se fornecida
        if (!string.IsNullOrWhiteSpace(knowledgeBase))
        {
            var knowledgeMessage = new ChatMessage
            {
                Role = "system",
                Content = $"Here is some FAQ information that might help:\n{knowledgeBase}"
            };
            messages.Add(knowledgeMessage);
            await _chatHistoryRepository.SaveMessageAsync(helpdeskId, knowledgeMessage);
        }


        // Adicionar mensagem de clarificação, se fornecida
        if (!string.IsNullOrEmpty(clarificationMessage))
        {
            var clarificationMessageObj = new ChatMessage
            {
                Role = "assistant",
                Content = clarificationMessage
            };
            messages.Add(clarificationMessageObj);
            await _chatHistoryRepository.SaveMessageAsync(helpdeskId, clarificationMessageObj);
            return clarificationMessage; // Retorna diretamente a mensagem de clarificação
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

