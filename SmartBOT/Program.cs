using SmartBOT;

var apiKey = "sk-svcacct-Dz-PhIMoOCoACwP9h_4ouXR9_lWUu_Ku4zrC9x5rmblELtMX9yjJ8dPJe3nBG136NVigT3BlbkFJkZKpyjD_rstXNAF3LbNlNvtQpLfflJktmFWsfas8Ige0ZDd1Zcaf2k6TsoE9Ud6tTV4A";



var HelpDeskService = new TeslaHelpDeskService();
var EmbeddingsService = new OpenAIEmbeddingsService();
var searchService = new TelaProjectSearchService();


Console.WriteLine("Welcome to the chat with Tesla Assistent ClaudIA! How can I Help you?");


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
        var embeddedQuestion = await EmbeddingsService.GetEmbeddingAsync(userMessage);

        //Console.WriteLine("Embedding question:");
        //Console.WriteLine(string.Join(", ", embeddedQuestion));


        var context = await searchService.SearchAsync(embeddedQuestion);
        foreach (var result in context)
        {
            Console.WriteLine($"Content: {result.Content}");
            Console.WriteLine($"Type: {result.Type}");
            Console.WriteLine($"Score: {result.Score}");
            Console.WriteLine("---");
        }


      
        //var teslaResponse = await teslaHelpDeskService.SendUserMessageAsync(userMessage);
        //Console.WriteLine($"ClaudiIA: {teslaResponse}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}
