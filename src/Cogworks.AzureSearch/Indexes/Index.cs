using Cogworks.AzureSearch.Interfaces.Indexes;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Indexes
{
    internal class Index<TModel> : IIndex<TModel> where TModel : class, IModel, new()
    {
        private readonly IDocumentOperation<TModel> _documentOperation;

        public Index(IDocumentOperation<TModel> documentOperation)
            => _documentOperation = documentOperation;

        public async Task<DocumentOperationResult> AddOrUpdateDocumentAsync(TModel model)
            => await _documentOperation.AddOrUpdateDocumentAsync(model);

        public async Task<DocumentOperationResult> TryRemoveDocumentAsync(TModel model)
            => await _documentOperation.TryRemoveDocumentAsync(model);

        public async Task<BatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TModel> models)
            => await _documentOperation.AddOrUpdateDocumentsAsync(models);

        public async Task<BatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TModel> models)
            => await _documentOperation.TryRemoveDocumentsAsync(models);
    }
}