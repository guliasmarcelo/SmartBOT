using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace SmartBOT.WebAPI.Core;

/// <summary>
/// Classe stateless para realizar o chat com o agente de HelpDesk da Tesla.
/// Gerencia mensagens e histórico por meio de argumentos.
/// </summary>
public class OpenAIChatService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly string _connectionString; // Caminho do banco SQLite

    public OpenAIChatService(string databasePath = "chat_history.db")
    {
        var apiKey = "sk-svcacct-Dz-PhIMoOCoACwP9h_4ouXR9_lWUu_Ku4zrC9x5rmblELtMX9yjJ8dPJe3nBG136NVigT3BlbkFJkZKpyjD_rstXNAF3LbNlNvtQpLfflJktmFWsfas8Ige0ZDd1Zcaf2k6TsoE9Ud6tTV4A";

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.openai.com/v1/")
        };
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        _connectionString = $"Data Source={databasePath}";
        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_connectionString);
        connection.Open();

        var command = connection.CreateCommand();
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS ChatHistory (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                HelpdeskId TEXT NOT NULL,
                Role TEXT NOT NULL,
                Content TEXT NOT NULL,
                Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
            );
        ";
        command.ExecuteNonQuery();
    }

    public async Task<List<ChatMessage>> LoadChatHistoryAsync(string helpdeskId)
    {
        var messages = new List<ChatMessage>();

        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            SELECT Role, Content
            FROM ChatHistory
            WHERE HelpdeskId = @helpdeskId
            ORDER BY Timestamp;
        ";
        command.Parameters.AddWithValue("@helpdeskId", helpdeskId);

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            messages.Add(new ChatMessage
            {
                Role = reader.GetString(0),
                Content = reader.GetString(1)
            });
        }

        return messages;
    }

    private async Task SaveMessageAsync(string helpdeskId, ChatMessage message)
    {
        using var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
            INSERT INTO ChatHistory (HelpdeskId, Role, Content)
            VALUES (@helpdeskId, @role, @content);
        ";
        command.Parameters.AddWithValue("@helpdeskId", helpdeskId);
        command.Parameters.AddWithValue("@role", message.Role);
        command.Parameters.AddWithValue("@content", message.Content);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<string> SendUserMessageAsync(
     string helpdeskId,
     List<ChatMessage> messages,
     string userMessage,
     string knowledgeBase,
     string model,
     string systemMessage)
    {
        // Adicionar a mensagem de sistema na primeira interação
        if (!messages.Any())
        {
            messages.Add(new ChatMessage
            {
                Role = "system",
                Content = systemMessage
            });

            // Salvar a mensagem de sistema no histórico
            await SaveMessageAsync(helpdeskId, new ChatMessage
            {
                Role = "system",
                Content = systemMessage
            });
        }

        // Adicionar FAQ como mensagem do sistema, se fornecida
        if (!string.IsNullOrWhiteSpace(knowledgeBase))
        {
            var knolodgeMessmessage = new ChatMessage 
            {
                Role = "system",
                Content = $"Here is some FAQ information that might help:\n{knowledgeBase}"
            };
            messages.Add(knolodgeMessmessage);
            await SaveMessageAsync(helpdeskId, knolodgeMessmessage);
        }

        // Adicionar mensagem do usuário
        var userMessageObj = new ChatMessage { Role = "user", Content = userMessage };
        messages.Add(userMessageObj);
        await SaveMessageAsync(helpdeskId, userMessageObj);

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
            await SaveMessageAsync(helpdeskId, assistantMessageObj);
        }

        return assistantMessage ?? throw new Exception("Error when trying to answer the question!");
    }

}
