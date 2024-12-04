namespace SmartBOT.WebAPI.Core;

public interface IVectorDbSearchService
{
    Task<List<SearchResult>> SearchAsync(float[] embeddings, string projectName, int top, int k);
}