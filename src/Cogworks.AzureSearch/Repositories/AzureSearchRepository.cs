using System.Collections.Generic;
using System.Threading.Tasks;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Repositories;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Repositories
{
    internal class AzureSearchRepository<TAzureModel> : IAzureSearchRepository<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
        private readonly IAzureIndexOperation<TAzureModel> _indexOperation;
        private readonly IAzureDocumentOperation<TAzureModel> _documentOperation;
        private readonly IAzureSearch<TAzureModel> _search;

        public AzureSearchRepository(
            IAzureIndexOperation<TAzureModel> indexOperation,
            IAzureDocumentOperation<TAzureModel> documentOperation,
            IAzureSearch<TAzureModel> search)
        {
            _indexOperation = indexOperation;
            _documentOperation = documentOperation;
            _search = search;
        }

        public async Task<AzureDocumentOperationResult> AddOrUpdateDocumentAsync(TAzureModel model)
            => await _documentOperation.AddOrUpdateDocumentAsync(model);

        public async Task<AzureBatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _documentOperation.AddOrUpdateDocumentsAsync(models);

        public async Task<AzureDocumentOperationResult> TryRemoveDocumentAsync(TAzureModel model)
            => await _documentOperation.TryRemoveDocumentAsync(model);

        public async Task<AzureBatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _documentOperation.TryRemoveDocumentsAsync(models);

        public async Task<bool> IndexExistsAsync()
            => await _indexOperation.IndexExistsAsync();

        public async Task IndexDeleteAsync()
            => await _indexOperation.IndexDeleteAsync();

        public async Task IndexCreateOrUpdateAsync()
            => await _indexOperation.IndexCreateOrUpdateAsync();

        public async Task IndexClearAsync()
            => await _indexOperation.IndexClearAsync();

        public SearchResult<TAzureModel> Search(string keyword, AzureSearchParameters azureSearchParameters)
            => _search.Search(keyword, azureSearchParameters);

        public async Task<SearchResult<TAzureModel>> SearchAsync(string keyword,
            AzureSearchParameters azureSearchParameters)
            => await _search.SearchAsync(keyword, azureSearchParameters);
    }
}