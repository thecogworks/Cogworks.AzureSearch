using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Wrappers
{
    internal class IndexOperationWrapper : IIndexOperationWrapper
    {
        private readonly IIndexesOperations _indexOperation;

        public IndexOperationWrapper(AzureSearchClientOption azureSearchClientOption)
            => _indexOperation = azureSearchClientOption.GetSearchServiceClient().Indexes;

        public async Task<bool> ExistsAsync(string indexName)
            => await _indexOperation.ExistsAsync(indexName);

        public async Task DeleteAsync(string indexName)
            => await _indexOperation.DeleteAsync(indexName);

        public async Task<Index> CreateOrUpdateAsync<TAzureModel>(string indexName) where TAzureModel : class, IAzureModel, new()
        {
            var indexDefinition = new Index
            {
                Name = indexName,
                Fields = FieldBuilder.BuildForType<TAzureModel>(),
                
            };

            return await _indexOperation.CreateOrUpdateAsync(indexDefinition);
        }

        public async Task<Index> CreateOrUpdateAsync<TAzureModel>(Index customIndexDefinition, bool overrideFields = true) where TAzureModel : class, IAzureModel, new()
        {
            if (overrideFields)
            {
                customIndexDefinition.Fields = FieldBuilder.BuildForType<TAzureModel>();
            }

            return await _indexOperation.CreateOrUpdateAsync(customIndexDefinition);
        } 
    }
}