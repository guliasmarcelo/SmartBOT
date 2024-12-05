namespace SmartBOT.WebAPI.Core;

public interface IChatService
{
    Task<List<ChatMessage>> LoadChatHistoryAsync(string helpdeskId);
    Task<string> SendUserMessageAsync(string helpdeskId, List<ChatMessage> messages, string userMessage, string knowledgeBase, string model, string InstructionsMessage, string? clarificationMessage = null);
}