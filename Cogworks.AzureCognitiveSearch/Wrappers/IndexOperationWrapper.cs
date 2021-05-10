using System;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureCognitiveSearch.Interfaces.Wrappers;
using Cogworks.AzureCognitiveSearch.Models;
using Cogworks.AzureCognitiveSearch.Options;

namespace Cogworks.AzureCognitiveSearch.Wrappers
{
    internal class IndexOperationWrapper : IIndexOperationWrapper
    {
        private readonly SearchIndexClient _searchIndexClient;

        public IndexOperationWrapper(AzureSearchClientOption azureSearchClientOption)
        {
            var azureKeyCredential = new AzureKeyCredential(azureSearchClientOption.Credentials);

            _searchIndexClient = new SearchIndexClient(
                endpoint: new Uri(azureSearchClientOption.ServiceUrlEndpoint),
                credential: azureKeyCredential);
        }

        public async Task<bool> ExistsAsync(string indexName)
            => (await _searchIndexClient.GetIndexAsync(indexName)).Value != null;

        public async Task DeleteAsync(string indexName)
            => await _searchIndexClient.DeleteIndexAsync(indexName);

        public async Task<SearchIndex> CreateOrUpdateAsync<TAzureModel>(string indexName) where TAzureModel : class, IAzureModel, new()
        {
            var fieldBuilder = new FieldBuilder();
            var searchFields = fieldBuilder.Build(typeof(TAzureModel));

            var definition = new SearchIndex(
                indexName,
                searchFields);

            return await _searchIndexClient.CreateOrUpdateIndexAsync(definition);
        }

        public async Task<SearchIndex> CreateOrUpdateAsync<TAzureModel>(SearchIndex customIndexDefinition, bool overrideFields) where TAzureModel : class, IAzureModel, new()
        {
            if (overrideFields)
            {
                var fieldBuilder = new FieldBuilder();
                var searchFields = fieldBuilder.Build(typeof(TAzureModel));

                customIndexDefinition.Fields = searchFields;
            }

            return await _searchIndexClient.CreateOrUpdateIndexAsync(customIndexDefinition);
        }
    }
}