using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBOT
{
    public class TeslaHelpDeskIntegrationService
    {
        private readonly OpenAIEmbeddingsService _embeddingsService;
        private readonly AzureAISearchService _searchService;
        private readonly OpenAIChatService _chatService;

        public TeslaHelpDeskIntegrationService()
        {
            var systemMessage = @"  
You are ClaudIA, a support assistant specialized in answering questions about Tesla Motors and its products. 
Your responses must be concise, using simple language, and limited to no more than three sentences.
If the user asks about anything unrelated to Tesla Motors or its products, politely inform them that you can only provide information about Tesla Motors.
            ";

            _chatService = new OpenAIChatService(systemMessage);
            _embeddingsService = new OpenAIEmbeddingsService();
            _searchService = new AzureAISearchService();
        }

        /// <summary>
        /// Classe responsável por orquestrar os serviços com o objetivo de responder o usuário
        /// </summary>
        /// <param name="userMessage">User Message</param>
        /// <returns>The ClaudIA respose to the user</returns>
        public async Task<string> HandleUserQueryAsync(string userMessage)
        {
         
            // Search from KnowLedgeBase
            var embeddedQuestion = await _embeddingsService.GetEmbeddingAsync(userMessage);            
            var searchResults = await _searchService.SearchAsync(embeddedQuestion, "tesla_motors", 10, 3);
            if (!searchResults.Any())
            {
                throw new Exception("Any results from vectorial search!");
            }

            var knowledgeBase = string.Join("\n", searchResults.Select(r => $"- {r.Content} (Type: {r.Type}, Score: {r.Score:F2})"));

            // Sends the objects to the chat 
            var response = await _chatService.SendUserMessageAsync(userMessage, knowledgeBase, "gpt-4o");

            return response;
        }
    }
}
