﻿using Microsoft.Data.Sqlite;


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
                    UserMessage TEXT NOT NULL,
                    AssistantResponse TEXT NOT NULL,
                    Timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
                );
            ";
        command.ExecuteNonQuery();
    }

    public async Task SaveMessageAsync(string userMessage, string assistantResponse)
    {
        using var connection = new SqliteConnection(_SqLiteConnectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
                INSERT INTO ChatHistory (UserMessage, AssistantResponse)
                VALUES (@userMessage, @assistantResponse);
            ";
        command.Parameters.AddWithValue("@userMessage", userMessage);
        command.Parameters.AddWithValue("@assistantResponse", assistantResponse);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<List<(string UserMessage, string AssistantResponse, DateTime Timestamp)>> GetHistoryAsync()
    {
        var history = new List<(string UserMessage, string AssistantResponse, DateTime Timestamp)>();

        using var connection = new SqliteConnection(_SqLiteConnectionString);
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = "SELECT UserMessage, AssistantResponse, Timestamp FROM ChatHistory ORDER BY Timestamp";

        using var reader = await command.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            history.Add((
                reader.GetString(0),
                reader.GetString(1),
                reader.GetDateTime(2)
            ));
        }

        return history;
    }
}
