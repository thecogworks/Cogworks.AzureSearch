using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using Cogworks.AzureSearch.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Indexes
{
    public interface IAzureIndex<in TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        Task AddOrUpdateDocumentAsync(TAzureModel model);

        Task AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models);

        Task<AzureRemoveResultDto> TryRemoveDocumentAsync(TAzureModel model);

        Task<IEnumerable<AzureRemoveResultDto>> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models);
    }

    public class AzureIndex<TAzureModel> : IAzureIndex<TAzureModel> where TAzureModel : class, IAzureModelIdentity, new()
    {
        private readonly IAzureDocumentOperation<TAzureModel> _azureSearchRepository;

        public AzureIndex(IAzureDocumentOperation<TAzureModel> azureSearchRepository)
            => _azureSearchRepository = azureSearchRepository;

        public async Task AddOrUpdateDocumentAsync(TAzureModel model)
            => await _azureSearchRepository.AddOrUpdateDocumentAsync(model);

        public async Task<AzureRemoveResultDto> TryRemoveDocumentAsync(TAzureModel model)
            => await _azureSearchRepository.TryRemoveDocumentAsync(model);

        public async Task AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _azureSearchRepository.AddOrUpdateDocumentsAsync(models);

        public async Task<IEnumerable<AzureRemoveResultDto>> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _azureSearchRepository.TryRemoveDocumentsAsync(models);
    }
}