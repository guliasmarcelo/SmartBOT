namespace SmartBOT.WebAPI.Core;

public interface IEmbeddingsService
{
    Task<float[]> GetEmbeddingAsync(string inputText);
}