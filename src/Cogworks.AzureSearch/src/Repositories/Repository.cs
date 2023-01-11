using System.Collections.Generic;
using System.Threading.Tasks;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Repositories;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Repositories
{
    internal class Repository<TModel> : IRepository<TModel>
        where TModel : class, IModel, new()
    {
        private readonly IIndexOperation<TModel> _indexOperation;
        private readonly IDocumentOperation<TModel> _documentOperation;
        private readonly ISearcher<TModel> _search;

        public Repository(
            IIndexOperation<TModel> indexOperation,
            IDocumentOperation<TModel> documentOperation,
            ISearcher<TModel> search)
        {
            _indexOperation = indexOperation;
            _documentOperation = documentOperation;
            _search = search;
        }

        public async Task<DocumentOperationResult> AddOrUpdateDocumentAsync(TModel model)
            => await _documentOperation.AddOrUpdateDocumentAsync(model);

        public async Task<BatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TModel> models)
            => await _documentOperation.AddOrUpdateDocumentsAsync(models);

        public async Task<DocumentOperationResult> TryRemoveDocumentAsync(TModel model)
            => await _documentOperation.TryRemoveDocumentAsync(model);

        public async Task<BatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TModel> models)
            => await _documentOperation.TryRemoveDocumentsAsync(models);

        public async Task<bool> IndexExistsAsync()
            => await _indexOperation.IndexExistsAsync();

        public async Task IndexDeleteAsync()
            => await _indexOperation.IndexDeleteAsync();

        public async Task IndexCreateOrUpdateAsync()
            => await _indexOperation.IndexCreateOrUpdateAsync();

        public async Task IndexClearAsync()
            => await _indexOperation.IndexClearAsync();

        public SearchResult<TModel> Search(string keyword, SearchParameters searchParameters)
            => _search.Search(keyword, searchParameters);

        public async Task<SearchResult<TModel>> SearchAsync(string keyword,
            SearchParameters searchParameters)
            => await _search.SearchAsync(keyword, searchParameters);
    }
}