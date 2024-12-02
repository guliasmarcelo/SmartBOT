using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;



namespace SmartBOT;


/// <summary>
/// Classe resonsável por realizar a chat com o agente de HelpDesk da Tesla
/// </summary>
public class OpenAIChatService 
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly List<object> _messages; // message histories


    public OpenAIChatService(string systemMessage)
    {
        var apiKey = "sk-svcacct-Dz-PhIMoOCoACwP9h_4ouXR9_lWUu_Ku4zrC9x5rmblELtMX9yjJ8dPJe3nBG136NVigT3BlbkFJkZKpyjD_rstXNAF3LbNlNvtQpLfflJktmFWsfas8Ige0ZDd1Zcaf2k6TsoE9Ud6tTV4A";


        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.openai.com/v1/")
        };

        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

        // Set toCamelCase, expected format from OpenAI.
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        // Inicializa o histórico com uma mensagem do sistema
        _messages = new List<object>()
        {
            new 
            {
                role = "system",
                content = systemMessage
            }
        };
    }

    /// <summary>
    /// Envia uma mensagem do usuário com base de conhecimento adicional
    /// </summary>
    /// <param name="persona"></param>
    /// <param name="userMessage"></param>
    /// <param name="knowledgeBase"></param>
    /// <param name="model"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    /// 
    public async Task<string> SendUserMessageAsync(string userMessage, string knowledgeBase, string model)
    {

        // Adiciona a base de conhecimento como uma mensagem adicional do tipo "system"
        if (!string.IsNullOrWhiteSpace(knowledgeBase))
        {
            _messages.Add(new
            {
                role = "system",
                content = $"Here is some FAQ information that might help:\n{knowledgeBase}"
            });
        }
        
        // Adiciona a mensagem do usuário ao histórico
        _messages.Add(new { role = "user", content = userMessage });

        // Cria o corpo da requisição com o histórico completo
        var requestBody = new
        {
            model = model,
            messages = _messages
        };

        // Envia a requisição
        var content = new StringContent(JsonSerializer.Serialize(requestBody, _jsonOptions), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("chat/completions", content);

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadAsStringAsync();
            throw new Exception($"Erro na API: {error}");
        }

        // Processa a resposta
        var responseBody = await response.Content.ReadAsStringAsync();
        using var jsonDocument = JsonDocument.Parse(responseBody);
        var assistantMessage = jsonDocument.RootElement
            .GetProperty("choices")[0]
            .GetProperty("message")
            .GetProperty("content")
            .GetString();

        // Adiciona a resposta do assistente ao histórico
        _messages.Add(new { role = "assistant", content = assistantMessage });

        return assistantMessage ?? throw new Exception("Error when trying to answear the question!");
    }

}

