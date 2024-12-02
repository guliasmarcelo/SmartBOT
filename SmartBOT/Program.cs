using SmartBOT;

var apiKey = "sk-svcacct-Dz-PhIMoOCoACwP9h_4ouXR9_lWUu_Ku4zrC9x5rmblELtMX9yjJ8dPJe3nBG136NVigT3BlbkFJkZKpyjD_rstXNAF3LbNlNvtQpLfflJktmFWsfas8Ige0ZDd1Zcaf2k6TsoE9Ud6tTV4A";



var openAIService = new OpenAIService(apiKey);

Console.WriteLine("Welcome to the chat with Tesla Assistent ClaudIA!");



while (true)
{
    Console.Write("You: ");
    var userMessage = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userMessage) || userMessage.ToLower() == "exit")
    {
        Console.WriteLine("Closing the chat. See you!");
        break;
    }

    try
    {
        var response = await openAIService.SendChatMessageAsync("gpt-4o", userMessage);
        Console.WriteLine($"ClaudiIA: {response}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
