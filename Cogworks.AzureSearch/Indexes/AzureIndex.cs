using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using Cogworks.AzureSearch.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Indexes
{
    public interface IAzureIndex<in TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        Task<AzureDocumentOperationResultDto> AddOrUpdateDocumentAsync(TAzureModel model);

        Task<AzureBatchDocumentsOperationResultDto> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models);

        Task<AzureDocumentOperationResultDto> TryRemoveDocumentAsync(TAzureModel model);

        Task<AzureBatchDocumentsOperationResultDto> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models);
    }

    public class AzureIndex<TAzureModel> : IAzureIndex<TAzureModel> where TAzureModel : class, IAzureModelIdentity, new()
    {
        private readonly IAzureDocumentOperation<TAzureModel> _azureSearchRepository;

        public AzureIndex(IAzureDocumentOperation<TAzureModel> azureSearchRepository)
            => _azureSearchRepository = azureSearchRepository;

        public async Task<AzureDocumentOperationResultDto> AddOrUpdateDocumentAsync(TAzureModel model)
            => await _azureSearchRepository.AddOrUpdateDocumentAsync(model);

        public async Task<AzureDocumentOperationResultDto> TryRemoveDocumentAsync(TAzureModel model)
            => await _azureSearchRepository.TryRemoveDocumentAsync(model);

        public async Task<AzureBatchDocumentsOperationResultDto> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _azureSearchRepository.AddOrUpdateDocumentsAsync(models);

        public async Task<AzureBatchDocumentsOperationResultDto> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _azureSearchRepository.TryRemoveDocumentsAsync(models);
    }
}