using System;
using System.Threading.Tasks;
using Azure;
using Azure.Identity;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;

namespace Cogworks.AzureSearch.Wrappers
{
    internal class DocumentOperationWrapper<TModel> : IDocumentOperationWrapper<TModel>
        where TModel : class, IModel, new()
    {
        private readonly SearchClient _searchClient;

        public DocumentOperationWrapper(
            IndexDefinition<TModel> indexDefinition,
            ClientOption clientOption)
        {
            if (clientOption.UseTokenCredentials)
            {
                var azureCredential = new DefaultAzureCredential();

                _searchClient = new SearchClient(
                    new Uri(clientOption.ServiceUrlEndpoint),
                    indexDefinition.IndexName,
                    azureCredential,
                    clientOption.ClientOptions
                );
            }
            else
            {
                var azureKeyCredential = new AzureKeyCredential(clientOption.Credentials);

                _searchClient = new SearchClient(
                    endpoint: new Uri(clientOption.ServiceUrlEndpoint),
                    indexName: indexDefinition.IndexName,
                    credential: azureKeyCredential,
                    options: clientOption.ClientOptions
                );
            }
        }

        public SearchResults<TModel> Search(string searchText, SearchOptions parameters = null)
            => _searchClient.Search<TModel>(searchText, parameters);

        public async Task<SearchResults<TModel>> SearchAsync(string searchText, SearchOptions parameters = null)
            => await _searchClient.SearchAsync<TModel>(searchText, parameters);

        public async Task<Response<IndexDocumentsResult>> IndexAsync(IndexDocumentsBatch<TModel> indexBatch)
            => await _searchClient.IndexDocumentsAsync(indexBatch);
    }
}