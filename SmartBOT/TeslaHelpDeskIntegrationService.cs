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
        private readonly TelaProjectSearchService _searchService;
        private readonly TeslaHelpDeskService _helpDeskService;

        public TeslaHelpDeskIntegrationService()
        {
            _searchService = new TelaProjectSearchService();
            _helpDeskService = new TeslaHelpDeskService();
            _embeddingsService = new OpenAIEmbeddingsService();
        }

        /// <summary>
        /// Classe responsável por orquestrar a requisição do usuário
        /// </summary>
        /// <param name="userQuestion"></param>
        /// <returns></returns>
        public async Task<string> HandleUserQueryAsync(string userQuestion)
        {
            // Passo 1: Embedar a pergunta do cliente
            var embeddedQuestion = await _embeddingsService.GetEmbeddingAsync(userQuestion);

            // Passo 2: Buscar a base de conhecimento banco vetorial
            var searchResults = await _searchService.SearchAsync(embeddedQuestion);

            // Passo 2: Construir o prompt baseado nos resultados
            var baseKnowledge = string.Join("\n", searchResults.Select(r => $"- {r.Content}"));
            var prompt = $"Here is some information that might help:\n{baseKnowledge}\n\nUser's Question: {userQuestion}\n";

            // Passo 3: Enviar o prompt para o TeslaHelpDeskService
            var response = await _helpDeskService.SendPromptAsync(prompt);

            return response;
        }
    }
}
