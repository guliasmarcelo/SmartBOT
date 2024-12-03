using SmartBOT;

// Instanciar o serviço de integração
var integrationService = new TeslaHelpDeskIntegrationService();

// Criar uma nova conversa
var helpdeskId = Guid.NewGuid().ToString();


Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Welcome to the chat with Tesla Assistant ClaudIA! How can I help you?");
Console.WriteLine($"Conversation ID: {helpdeskId}");

while (true)
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.Write("You: ");
    var userQuestion = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userQuestion) || userQuestion.ToLower() == "exit")
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("Closing the chat. See you!");
        break;
    }

    try
    {
        // Enviar a mensagem para o assistente via integração
        var response = await integrationService.HandleUserQueryAsync(helpdeskId, userQuestion);

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Resposta da ClaudIA:");
        Console.WriteLine(response);
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error: {ex.Message}");
    }
}


