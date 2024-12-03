namespace SmartBOT.WebAPI.Core;

public interface IVectorSearchService
{
    Task<List<SearchResult>> SearchAsync(float[] embeddings, string projectName, int top, int k);
}