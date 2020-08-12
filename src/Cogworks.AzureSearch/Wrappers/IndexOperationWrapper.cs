using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Wrappers
{
    public interface IIndexOperationWrapper
    {
        Task<bool> ExistsAsync(string indexName);

        Task DeleteAsync(string indexName);

        Task<Index> CreateOrUpdateAsync<TAzureModel>(string indexName) where TAzureModel : class, IAzureModel, new();
    }

    public class IndexOperationWrapper : IIndexOperationWrapper
    {
        private readonly IIndexesOperations _indexOperation;

        public IndexOperationWrapper(AzureSearchClientOption azureSearchClientOption)
            => _indexOperation = azureSearchClientOption.GetSearchServiceClient().Indexes;

        public async Task<bool> ExistsAsync(string indexName)
            => await _indexOperation.ExistsAsync(indexName);

        public async Task DeleteAsync(string indexName)
            => await _indexOperation.DeleteAsync(indexName);

        public async Task<Index> CreateOrUpdateAsync<TAzureModel>(string indexName) where TAzureModel : class, IAzureModel, new()
            => await _indexOperation.CreateOrUpdateAsync(new Index
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<TAzureModel>()
            });
    }
}