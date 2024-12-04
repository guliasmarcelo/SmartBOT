namespace SmartBOT.WebAPI.Core;

public interface ISearchService
{
    Task<List<SearchResult>> SearchAsync(float[] embeddings, string projectName, int top, int k);
}