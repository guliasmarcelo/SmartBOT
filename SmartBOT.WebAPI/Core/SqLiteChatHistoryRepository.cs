using Microsoft.Data.Sqlite;

namespace SmartBOT.WebAPI.Core;

public class SqLiteChatHistoryRepository : IChatHistoryRepository
{
    private readonly string? _SqLiteConnectionString;

    public SqLiteChatHistoryRepository(IConfiguration configuration)
    {
        _SqLiteConnectionString = configuration["ConnectionStrings:SqLiteConnectionString"];
        if (string.IsNullOrEmpty(_SqLiteConnectionString))
        {
            throw new Exception("SqLiteConnectionString not found in configuration.");
        }

        InitializeDatabase();
    }

    private void InitializeDatabase()
    {
        using var connection = new SqliteConnection(_SqLiteConnectionString);
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

        using var connection = new SqliteConnection(_SqLiteConnectionString);
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

    public async Task SaveMessageAsync(string helpdeskId, ChatMessage message)
    {
        using var connection = new SqliteConnection(_SqLiteConnectionString);
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
}
