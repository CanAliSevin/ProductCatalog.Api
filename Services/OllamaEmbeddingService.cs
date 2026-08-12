using System.Net.Http.Json;
using Pgvector;
using ProductCatalog.Api.Services;

namespace ProductCatalog.Api.Services
{

    public class OllamaEmbeddingService : IEmbeddingService
    {
        private readonly HttpClient _httpClient;

        public OllamaEmbeddingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Vector> GetEmbeddingAsync(string text)

        {
            var request = new
            {
                model = "embeddinggemma:300m", // Ollama embedding model
                input = text
            };

            var response = await _httpClient.PostAsJsonAsync("api/embed", request);

            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaEmbeddingResponse>();

            if (result?.Embeddings == null || result.Embeddings.Count == 0)
            {
                throw new Exception("Ollama embedding döndürmedi");
            }



            return new Vector(result.Embeddings[0]);
        }
        private class OllamaEmbeddingResponse
        {
            public List<float[]> Embeddings { get; set; } = [];
        }
    }


}