namespace SmartBOT.WebAPI.Core;

public interface IHelpDeskService
{
    Task<string> HandleUserQueryAsync(string helpdeskId, string userMessage);
}