using SmartBOT;

var integrationService = new TeslaHelpDeskIntegrationService();

Console.ForegroundColor = ConsoleColor.Green;

// Mostrar título com efeito de digitação
string title = "Welcome to SmartBOT!";
TypeEffect(title, delay: 100);

Console.WriteLine("\n");

// Mostrar animação de carregamento
Console.ForegroundColor = ConsoleColor.Yellow;
LoadingAnimation("Initializing", 3);

// Finalizar abertura
Console.ResetColor();
Console.WriteLine("\nApplication is ready!");

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("Welcome to the chat with Tesla Assistent ClaudIA! How can I Help you?");

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
        var response = await integrationService.HandleUserQueryAsync(userQuestion);
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


// Método para efeito de digitação
static void TypeEffect(string text, int delay = 50)
{
    foreach (char c in text)
    {
        Console.Write(c);
        Thread.Sleep(delay); // Atraso entre os caracteres
    }
}

// Método para animação de carregamentostatic void LoadingAnimation(string text, int dots = 3, int delay = 500)
static void LoadingAnimation(string text, int dots = 3, int delay = 500)
{
    for (int i = 0; i < dots; i++)
    {
        Console.Write($"\r{text}{new string('.', i + 1)}   "); // Atualiza a linha com mais pontos
        Thread.Sleep(delay);
    }
    Console.Write("\r{text}... Done!"); // Mensagem final
}
