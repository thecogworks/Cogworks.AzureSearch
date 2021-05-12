
using System;
using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;

namespace Cogworks.AzureSearch.Wrappers
{
    internal class DocumentOperationWrapper<TAzureModel> : IDocumentOperationWrapper<TAzureModel>
        where TAzureModel : class, IModel, new()
    {
        private readonly SearchClient _searchClient;

        public DocumentOperationWrapper(
            IndexDefinition<TAzureModel> indexDefinition,
            ClientOption clientOption)
        {
            var azureKeyCredential = new AzureKeyCredential(clientOption.Credentials);

            _searchClient = new SearchClient(
                endpoint: new Uri(clientOption.ServiceUrlEndpoint),
                indexName: indexDefinition.IndexName,
                credential: azureKeyCredential,
                options: clientOption.ClientOptions

            );
        }

        public SearchResults<TAzureModel> Search(string searchText, SearchOptions parameters = null)
            => _searchClient.Search<TAzureModel>(searchText, parameters);

        public async Task<SearchResults<TAzureModel>> SearchAsync(string searchText, SearchOptions parameters = null)
            => await _searchClient.SearchAsync<TAzureModel>(searchText, parameters);

        public async Task<Response<IndexDocumentsResult>> IndexAsync(IndexDocumentsBatch<TAzureModel> indexBatch)
            => await _searchClient.IndexDocumentsAsync(indexBatch);
    }
}