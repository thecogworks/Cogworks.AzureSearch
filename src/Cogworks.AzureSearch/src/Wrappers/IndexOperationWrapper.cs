using System;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents.Indexes;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;

namespace Cogworks.AzureSearch.Wrappers
{
    internal class IndexOperationWrapper : IIndexOperationWrapper
    {
        private readonly SearchIndexClient _searchIndexClient;

        public IndexOperationWrapper(ClientOption clientOption)
        {
            var azureKeyCredential = new AzureKeyCredential(clientOption.Credentials);

            _searchIndexClient = new SearchIndexClient(
                endpoint: new Uri(clientOption.ServiceUrlEndpoint),
                credential: azureKeyCredential);
        }

        public async Task<bool> ExistsAsync(string indexName)
            => (await _searchIndexClient.GetIndexAsync(indexName)).Value != null;

        public async Task DeleteAsync(string indexName)
            => await _searchIndexClient.DeleteIndexAsync(indexName);

        public async Task<SearchIndex> CreateOrUpdateAsync<TModel>(string indexName)
            where TModel : class, IModel, new()
        {
            var fieldBuilder = new FieldBuilder();
            var searchFields = fieldBuilder.Build(typeof(TModel));

            var definition = new SearchIndex(
                indexName,
                searchFields);

            return await _searchIndexClient.CreateOrUpdateIndexAsync(definition);
        }

        public async Task<SearchIndex> CreateOrUpdateAsync<TModel>(SearchIndex customIndexDefinition, bool overrideFields)
            where TModel : class, IModel, new()
        {
            if (overrideFields)
            {
                var fieldBuilder = new FieldBuilder();
                var searchFields = fieldBuilder.Build(typeof(TModel));

                customIndexDefinition.Fields = searchFields;
            }

            return await _searchIndexClient.CreateOrUpdateIndexAsync(customIndexDefinition);
        }
    }
}