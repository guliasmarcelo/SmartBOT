
namespace SmartBOT.WebAPI.Core
{
    public interface IChatHistoryRepository
    {
        Task<List<ChatMessage>> LoadChatHistoryAsync(string helpdeskId);
        Task SaveMessageAsync(string helpdeskId, ChatMessage message);
    }
}