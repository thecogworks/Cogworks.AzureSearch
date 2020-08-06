using Microsoft.Azure.Search;

namespace Cogworks.AzureSearch.Wrappers
{
    public interface IAzureSearchWrapper
    {
        ISearchIndexClient GetIndexClient(string indexName);
    }

    public class AzureSearchWrapper : IAzureSearchWrapper
    {
        private readonly ISearchServiceClient _serviceClient;

        public AzureSearchWrapper(string searchServiceName, string searchCredentials)
            => _serviceClient = new SearchServiceClient(
                searchServiceName,
                new SearchCredentials(searchCredentials));

        public ISearchIndexClient GetIndexClient(string indexName)
            => _serviceClient.Indexes.GetClient(indexName);
    }
}