
Console.WriteLine("Bem-vindo, posso te ajudar com alguma dúvida ou curiosidade sobre a Tesla!");
var apiKey = "sk-svcacct-Dz-PhIMoOCoACwP9h_4ouXR9_lWUu_Ku4zrC9x5rmblELtMX9yjJ8dPJe3nBG136NVigT3BlbkFJkZKpyjD_rstXNAF3LbNlNvtQpLfflJktmFWsfas8Ige0ZDd1Zcaf2k6TsoE9Ud6tTV4A";
var chatService = new ChatService1(apiKey);


while (true)
{
    Console.Write("Você: ");
    var userMessage = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userMessage) || userMessage.ToLower() == "sair")
    {
        Console.WriteLine("Encerrando o chat. Até mais!");
        break;
    }

    try
    {
        var response = await chatService.GetResponseAsync(userMessage);
        Console.WriteLine($"OpenAI: {response}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Erro: {ex.Message}");
    }
}
