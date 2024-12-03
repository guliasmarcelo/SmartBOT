using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;


var httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5053/") };


// Criar uma nova conversa
var helpdeskId = Guid.NewGuid().ToString();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Welcome to the chat with Tesla Assistant ClaudIA! How can I help you? Como?");
Console.WriteLine($"Conversation ID: {helpdeskId}");

while (true)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("You: ");
    var userMessage = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userMessage) || userMessage.ToLower() == "exit")
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Closing the chat. See you!");
        break;
    }

    try
    {
        // Enviar mensagem para a API
        var request = new { UserMessage = userMessage };
        var content = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
        var response = await httpClient.PostAsync($"api/chat/{helpdeskId}", content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            var chatResponse = JsonSerializer.Deserialize<ChatResponse>(result);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"ClaudIA: {chatResponse?.Response}");
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {response.StatusCode}");
        }
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {ex.Message}");
    }
}

public class ChatResponse
{
    [JsonPropertyName("response")]
    public string Response { get; set; }
}

