

namespace SmartBOT.WebAPI.Core;

/// <summary>
/// Serviço de integração para orquestrar chamadas entre serviços de embeddings, busca e chat.
/// </summary>
public class TeslaHelpDeskService : IHelpDeskService
{
    private readonly IChatService _chatService;
    private readonly IEmbeddingsService _embeddingsService;
    private readonly IVectorDbSearchService _vectorDbSearchService;

    // Mensagem de sistema para configurar o comportamento do agente
    private const string SystemMessage = @"  
You are SmartBOT, a support assistant dedicated to answering questions exclusively about Tesla Motors and its products. Your goal is to help users by leveraging a dynamic FAQ knowledge base extracted through semantic searches.

#### Rules:
1. You must only provide answers related to Tesla Motors and its products. Do not respond to questions outside this scope.
2. Your responses should be based strictly on the information available in the extracted FAQ knowledge base. If the knowledge base does not contain relevant information, kindly clarify with the user to better understand their question or redirect the query appropriately.
3. If unsure of an answer, use clarifications sparingly—up to two times per conversation. 
4. When providing clarification, always start the response with ""Clarification: "" (in English) or ""Clarificação: "" (in Portuguese). For example:
   - Clarification: I currently do not have detailed information on that.
   - Clarificação: Atualmente, não tenho informações detalhadas sobre isso.

#### Behavior:
1. Keep your language simple, clear, and user-friendly.
2. Respond in a concise manner, with a maximum of three paragraphs.
3. If the user's question is outside the scope of Tesla Motors, gently inform them that you are only equipped to handle questions related to Tesla Motors.

#### Knowledge Base Usage:
1. Use the most relevant FAQ entries based on their semantic similarity scores, prioritizing higher scores when crafting your response.
2. If the provided FAQ entries are insufficient for a confident response, ask the user for additional details or rephrase their query to ensure accuracy.
3. Inform the user when you rely on specific FAQ entries, e.g., ""Based on the information from our knowledge base: [FAQ].""

Remember, your primary objective is to provide accurate and helpful information about Tesla Motors, ensuring a positive and informative experience for the user.


";

    public TeslaHelpDeskService(IChatService chatService, IEmbeddingsService embeddingsService, IVectorDbSearchService vectorDbSearchService)
    {
        // Inicializar os serviços
        _chatService = chatService;
        _embeddingsService = embeddingsService;
        _vectorDbSearchService = vectorDbSearchService;
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

        // Verificar o número de clarificações já realizadas
        var clarificationCount = messages.Count(m => m.Role == "assistant" &&
                                                     (m.Content.StartsWith("Clarification: ") ||
                                                      m.Content.StartsWith("Clarificação: ")));



        if (clarificationCount >= 2)
        {
            var escalationMessage = "I'm sorry, but I cannot clarify further. Your query will be escalated to a human assistant. Thank you for your understanding.";
            await _chatService.SendUserMessageAsync(
                helpdeskId,
                messages,
                userMessage,
                string.Empty,
                "gpt-4o",
                SystemMessage,
                escalationMessage
            );
            return escalationMessage;
        }



        // Obter embedding da mensagem do usuário
        var embeddedQuestion = await _embeddingsService.GetEmbeddingAsync(userMessage);

        // Realizar busca vetorial na base de conhecimento
        var searchResults = await _vectorDbSearchService.SearchAsync(embeddedQuestion, "tesla_motors", 10, 3);

        



        // Formatar a base de conhecimento
        var knowledgeBase = FormatKnowledgeBase(searchResults);

        // Enviar mensagem para o assistente e obter a resposta
        var response = await _chatService.SendUserMessageAsync(helpdeskId, messages, userMessage, knowledgeBase, "gpt-4o", SystemMessage);

        return response;
    }

    private string FormatKnowledgeBase(IEnumerable<SearchResult> searchResults)
    {
        var formattedEntries = searchResults
            .OrderByDescending(r => r.Score) // Ordenar por pontuação decrescente
            .Select(r => $@"
#### FAQ Entry
Score: {r.Score:F2}
Type: {r.Type ?? "General"}
Content: {r.Content}
");

        return string.Join("\n", formattedEntries);
    }

}


