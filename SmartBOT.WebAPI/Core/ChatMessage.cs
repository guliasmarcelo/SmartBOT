namespace SmartBOT.WebAPI.Core;

public struct ChatMessage
{
    public string Role { get; set; } // Pode ser "system", "user" ou "assistant"
    public string Content { get; set; } // Conteúdo da mensagem
}
