using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureCognitiveSearch.Interfaces.Searches;
using Cogworks.AzureCognitiveSearch.Models;

namespace Cogworks.AzureCognitiveSearch.Builder
{
    public interface IAzureSearchBuilder
    {
        IAzureSearchBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false);

        IAzureSearchBuilder RegisterClientOptions(string serviceName, string credentials);

        IAzureSearchBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IAzureModel, new();

        IAzureSearchBuilder RegisterIndexDefinitions<TDocument>(SearchIndex customIndex)
            where TDocument : class, IAzureModel, new();

        IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IAzureModel, new()
            where TSearcher : class, IAzureSearch<TDocument>, TSearcherType
            where TSearcherType : class;

        IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(TSearcherType instance)
            where TDocument : class, IAzureModel, new()
            where TSearcher : class, IAzureSearch<TDocument>, TSearcherType
            where TSearcherType : class;
    }
}