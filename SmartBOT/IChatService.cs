
namespace SmartBOT
{
    public interface IChatService
    {
        Task<string> SendPromptAsync(string userMessage);
    }
}