using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Searchers;

namespace Cogworks.AzureSearch.Interfaces.Builder
{
    public interface IContainerBuilder
    {
        IContainerBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false);

        IContainerBuilder RegisterClientOptions(string serviceName, string credentials, string serviceEndpointUrl);

        IContainerBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IModel, new();

        IContainerBuilder RegisterIndexDefinitions<TDocument>(SearchIndex customIndex)
            where TDocument : class, IModel, new();

        IContainerBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class;

        IContainerBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(TSearcherType instance)
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class;
    }
}