namespace SmartBOT.WebAPI.Core;

public interface IChatHistoryRepository
{
    Task<List<(string UserMessage, string AssistantResponse, DateTime Timestamp)>> GetHistoryAsync();
    Task SaveMessageAsync(string userMessage, string assistantResponse);
}