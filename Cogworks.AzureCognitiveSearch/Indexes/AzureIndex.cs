using Cogworks.AzureCognitiveSearch.Interfaces.Indexes;
using Cogworks.AzureCognitiveSearch.Interfaces.Operations;
using Cogworks.AzureCognitiveSearch.Models;
using Cogworks.AzureCognitiveSearch.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cogworks.AzureCognitiveSearch.Indexes
{
    internal class AzureIndex<TAzureModel> : IAzureIndex<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        private readonly IAzureDocumentOperation<TAzureModel> _documentOperation;

        public AzureIndex(IAzureDocumentOperation<TAzureModel> documentOperation)
            => _documentOperation = documentOperation;

        public async Task<AzureDocumentOperationResult> AddOrUpdateDocumentAsync(TAzureModel model)
            => await _documentOperation.AddOrUpdateDocumentAsync(model);

        public async Task<AzureDocumentOperationResult> TryRemoveDocumentAsync(TAzureModel model)
            => await _documentOperation.TryRemoveDocumentAsync(model);

        public async Task<AzureBatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _documentOperation.AddOrUpdateDocumentsAsync(models);

        public async Task<AzureBatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _documentOperation.TryRemoveDocumentsAsync(models);
    }
}