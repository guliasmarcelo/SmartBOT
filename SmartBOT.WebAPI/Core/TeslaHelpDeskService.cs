

namespace SmartBOT.WebAPI.Core;

/// <summary>
/// Serviço de integração para orquestrar chamadas entre serviços de embeddings, busca e chat.
/// </summary>
public class TeslaHelpDeskService : IHelpDeskService
{
    private readonly OpenAIChatService _chatService;
    private readonly OpenAIEmbeddingsService _embeddingsService;
    private readonly AzureVectorSearchService _searchService;

    // Mensagem de sistema para configurar o comportamento do agente
    private const string SystemMessage = @"  
You are ClaudIA, a support assistant specialized in answering questions about Tesla Motors and its products. 
Your responses must be concise, using simple language, and limited to no more than three sentences.
If the user asks about anything unrelated to Tesla Motors or its products, politely inform them that you can only provide information about Tesla Motors.
    ";

    public TeslaHelpDeskService()
    {
        // Inicializar os serviços
        _chatService = new OpenAIChatService();
        _embeddingsService = new OpenAIEmbeddingsService();
        _searchService = new AzureVectorSearchService();
    }

    /// <summary>
    /// Orquestra o processo de busca vetorial e envio de mensagens para o chat.
    /// </summary>
    /// <param name="helpdeskId">Identificador único da conversa.</param>
    /// <param name="userMessage">Mensagem do usuário.</param>
    /// <returns>Resposta do assistente.</returns>
    public async Task<string> HandleUserQueryAsync(string helpdeskId, string userMessage)
    {
        // Carregar histórico de mensagens
        var messages = await _chatService.LoadChatHistoryAsync(helpdeskId);

        // Obter embedding da mensagem do usuário
        var embeddedQuestion = await _embeddingsService.GetEmbeddingAsync(userMessage);

        // Realizar busca vetorial na base de conhecimento
        var searchResults = await _searchService.SearchAsync(embeddedQuestion, "tesla_motors", 10, 3);

        // Validar resultados da busca
        if (!searchResults.Any())
        {
            throw new Exception("No relevant results found in the knowledge base.");
        }

        // Construir base de conhecimento para auxiliar a resposta
        var knowledgeBase = string.Join("\n", searchResults.Select(r => $"- {r.Content} (Score: {r.Score:F2})"));

        // Enviar mensagem para o assistente e obter a resposta
        var response = await _chatService.SendUserMessageAsync(helpdeskId, messages, userMessage, knowledgeBase, "gpt-4o", SystemMessage);

        return response;
    }
}
