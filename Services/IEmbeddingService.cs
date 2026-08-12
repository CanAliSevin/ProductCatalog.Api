using Pgvector;

namespace ProductCatalog.Api.Services
{
    public interface IEmbeddingService
    {
        Task<Vector> GetEmbeddingAsync(string text);//Bu metod, bir string parametre alır ve bu metni temsil eden bir Vector nesnesi döndürür. Bu sayede, metinlerin vektör temsilleri elde edilebilir ve veritabanında saklanabilir veya sorgulanabilir.
    }
}