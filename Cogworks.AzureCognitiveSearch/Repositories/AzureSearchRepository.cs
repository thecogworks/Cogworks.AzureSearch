using System.Collections.Generic;
using System.Threading.Tasks;
using Cogworks.AzureCognitiveSearch.Interfaces.Operations;
using Cogworks.AzureCognitiveSearch.Interfaces.Repositories;
using Cogworks.AzureCognitiveSearch.Models;
using Cogworks.AzureCognitiveSearch.Models.Dtos;

namespace Cogworks.AzureCognitiveSearch.Repositories
{
    internal class AzureSearchRepository<TAzureModel> : IAzureSearchRepository<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
        private readonly IAzureIndexOperation<TAzureModel> _indexOperation;
        private readonly IAzureDocumentOperation<TAzureModel> _documentOperation;

        public AzureSearchRepository(
            IAzureIndexOperation<TAzureModel> indexOperation,
            IAzureDocumentOperation<TAzureModel> documentOperation)
        {
            _indexOperation = indexOperation;
            _documentOperation = documentOperation;
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
    }
}