using SmartBOT;

var integrationService = new HelpDeskIntegrationService();

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

        //var embeddedQuestion = await EmbeddingsService.GetEmbeddingAsync(userMessage);
        //Console.WriteLine("Embedding question:");
        //Console.WriteLine(string.Join(", ", embeddedQuestion));

        //var context = await searchService.SearchAsync(embeddedQuestion);
        //foreach (var result in context)
        //{
        //    Console.WriteLine($"Content: {result.Content}");
        //    Console.WriteLine($"Type: {result.Type}");
        //    Console.WriteLine($"Score: {result.Score}");
        //    Console.WriteLine("---");
        //}


    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
