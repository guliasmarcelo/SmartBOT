using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartBOT
{
    public class HelpDeskIntegrationService
    {
        private readonly TelaProjectSearchService _searchService;
        private readonly TeslaHelpDeskService _helpDeskService;

        public HelpDeskIntegrationService()
        {
            _searchService = new TelaProjectSearchService();
            _helpDeskService = new TeslaHelpDeskService();
        }

        public async Task<string> HandleUserQueryAsync(string userQuestion, float[] embeddings)
        {
            // Passo 1: Buscar a base de conhecimento banco vetorial
            var searchResults = await _searchService.SearchAsync(embeddings);

            // Passo 2: Construir o prompt baseado nos resultados
            var baseKnowledge = string.Join("\n", searchResults.Select(r => $"- {r.Content}"));
            var prompt = $"Here is some information that might help:\n{baseKnowledge}\n\nUser's Question: {userQuestion}\n";

            // Passo 3: Enviar o prompt para o TeslaHelpDeskService
            var response = await _helpDeskService.SendPromptAsync(prompt);

            return response;
        }
    }
}
