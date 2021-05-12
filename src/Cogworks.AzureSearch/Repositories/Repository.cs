using System.Collections.Generic;
using System.Threading.Tasks;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Repositories;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Repositories
{
    internal class Repository<TAzureModel> : IRepository<TAzureModel>
        where TAzureModel : class, IModel, new()
    {
        private readonly IIndexOperation<TAzureModel> _indexOperation;
        private readonly IDocumentOperation<TAzureModel> _documentOperation;
        private readonly ISearcher<TAzureModel> _search;

        public Repository(
            IIndexOperation<TAzureModel> indexOperation,
            IDocumentOperation<TAzureModel> documentOperation,
            ISearcher<TAzureModel> search)
        {
            _indexOperation = indexOperation;
            _documentOperation = documentOperation;
            _search = search;
        }

        public async Task<DocumentOperationResult> AddOrUpdateDocumentAsync(TAzureModel model)
            => await _documentOperation.AddOrUpdateDocumentAsync(model);

        public async Task<BatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _documentOperation.AddOrUpdateDocumentsAsync(models);

        public async Task<DocumentOperationResult> TryRemoveDocumentAsync(TAzureModel model)
            => await _documentOperation.TryRemoveDocumentAsync(model);

        public async Task<BatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _documentOperation.TryRemoveDocumentsAsync(models);

        public async Task<bool> IndexExistsAsync()
            => await _indexOperation.IndexExistsAsync();

        public async Task IndexDeleteAsync()
            => await _indexOperation.IndexDeleteAsync();

        public async Task IndexCreateOrUpdateAsync()
            => await _indexOperation.IndexCreateOrUpdateAsync();

        public async Task IndexClearAsync()
            => await _indexOperation.IndexClearAsync();

        public SearchResult<TAzureModel> Search(string keyword, SearchParameters searchParameters)
            => _search.Search(keyword, searchParameters);

        public async Task<SearchResult<TAzureModel>> SearchAsync(string keyword,
            SearchParameters searchParameters)
            => await _search.SearchAsync(keyword, searchParameters);
    }
}