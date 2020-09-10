using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Wrappers
{
    internal class DocumentOperationWrapper<TAzureModel> : IDocumentOperationWrapper<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
        private readonly IDocumentsOperations _documentOperation;

        public DocumentOperationWrapper(AzureIndexDefinition<TAzureModel> azureIndexDefinition, AzureSearchClientOption azureSearchClientOption)
            => _documentOperation = azureSearchClientOption.GetSearchServiceClient()
                .Indexes
                .GetClient(azureIndexDefinition.IndexName)
                .Documents;

        public DocumentSearchResult<TAzureModel> Search(string searchText, SearchParameters parameters = null)
            => _documentOperation.Search<TAzureModel>(searchText, parameters);

        public async Task<DocumentSearchResult<TAzureModel>> SearchAsync(string searchText, SearchParameters parameters = null)
            => await _documentOperation.SearchAsync<TAzureModel>(searchText, parameters);

        public async Task<DocumentIndexResult> IndexAsync(IndexBatch<TAzureModel> indexBatch)
            => await _documentOperation.IndexAsync(indexBatch);
    }
}