using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Repositories
{
    public interface IAzureSearchRepository
    {
        Task<bool> IndexExistsAsync();

        Task IndexDeleteAsync();

        Task IndexCreateOrUpdateAsync(Index indexDefinition);

        Task IndexClearAsync(Index indexDefinition);
    }

    public interface IAzureSearchRepository<TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        Task AddOrUpdateDocumentAsync(TAzureModel model);

        Task AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models);
    }

    public class AzureSearchRepository<TAzureModel> : IAzureSearchRepository, IAzureSearchRepository<TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        private readonly ISearchIndexClient _searchIndex;

        public AzureSearchRepository(AzureIndex<TAzureModel> azureIndex, AzureSearchClientOption azureSearchClientOption)
        {
            var searchServiceClient = azureSearchClientOption.GetSearchServiceClient();
            var searchIndexClient = searchServiceClient.Indexes.GetClient(azureIndex.IndexName);

            _searchIndex = searchIndexClient;
        }

        public Task<bool> IndexExistsAsync() => throw new System.NotImplementedException();

        public Task IndexDeleteAsync() => throw new System.NotImplementedException();

        public Task IndexCreateOrUpdateAsync(Index indexDefinition) => throw new System.NotImplementedException();

        public Task IndexClearAsync(Index indexDefinition) => throw new System.NotImplementedException();

        public Task AddOrUpdateDocumentAsync(TAzureModel model) => throw new System.NotImplementedException();

        public Task AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models) => throw new System.NotImplementedException();
    }
}