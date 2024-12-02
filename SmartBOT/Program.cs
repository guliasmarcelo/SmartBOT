using SmartBOT;

var integrationService = new TeslaHelpDeskIntegrationService();

Console.WriteLine("Welcome to the chat with Tesla Assistent ClaudIA! How can I Help you?");

while (true)
{
    Console.Write("You: ");
    var userQuestion = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userQuestion) || userQuestion.ToLower() == "exit")
    {
        Console.WriteLine("Closing the chat. See you!");
        break;
    }


    try
    {
        var response = await integrationService.HandleUserQueryAsync(userQuestion);
        Console.WriteLine("Resposta da ClaudIA:");
        Console.WriteLine(response);

    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
