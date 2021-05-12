using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Searchers;

namespace Cogworks.AzureSearch.Builder
{
    public interface IAzureSearchBuilder
    {
        IAzureSearchBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false);

        IAzureSearchBuilder RegisterClientOptions(string serviceName, string credentials, string serviceEndpointUrl);

        IAzureSearchBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IAzureModel, new();

        IAzureSearchBuilder RegisterIndexDefinitions<TDocument>(SearchIndex customIndex)
            where TDocument : class, IAzureModel, new();

        IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IAzureModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class;

        IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(TSearcherType instance)
            where TDocument : class, IAzureModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class;
    }
}