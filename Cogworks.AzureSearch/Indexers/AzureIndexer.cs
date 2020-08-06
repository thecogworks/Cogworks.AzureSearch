using Cogworks.AzureSearch.Extensions;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using Cogworks.AzureSearch.Options;
using Cogworks.AzureSearch.Repositories;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Indexers
{
    public interface IAzureIndexer<in TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        Task AddOrUpdateDocumentAsync(TAzureModel model);

        Task AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models);

        Task<AzureBatchRemoveResultDto> TryRemoveDocumentAsync(TAzureModel model);

        Task<IEnumerable<AzureBatchRemoveResultDto>> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models);
    }

    public class AzureIndexer<TAzureModel> : IAzureIndexer<TAzureModel> where TAzureModel : class, IAzureModelIdentity, new()
    {
        private readonly ISearchIndexClient _searchIndex;
        private readonly AzureSearchRepository<TAzureModel> _azureSearchRepository;

        public AzureIndexer(AzureIndex<TAzureModel> azureIndex, AzureSearchClientOption azureSearchClientOption)
        {
            _searchIndex = azureSearchClientOption.GetSearchServiceClient().Indexes.GetClient(azureIndex.IndexName);

            _azureSearchRepository = new AzureSearchRepository<TAzureModel>(azureIndex, azureSearchClientOption);
        }

        public async Task AddOrUpdateDocumentAsync(TAzureModel model)
            => await AddOrUpdateDocumentsAsync(new List<TAzureModel> { model });

        public async Task<AzureBatchRemoveResultDto> TryRemoveDocumentAsync(TAzureModel model)
            => (await TryRemoveDocumentsAsync(new List<TAzureModel> { model })).FirstOrDefault();

        public async Task AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models)
        {
            if (!models.HasAny())
            {
                return;
            }

            var batchActions = models
                .Select(model => new IndexAction<TAzureModel>(model, IndexActionType.Upload))
                .ToList();

            var batch = IndexBatch.New(batchActions);

            try
            {
                await _searchIndex.Documents.IndexAsync(batch);
            }
            catch (IndexBatchException ex)
            {
                //                _logger.Error<AzureIndexer>(ex,
                //                    $"Failed to index some documents: {string.Join(", ", ex.IndexingResults.Where(x => !x.Succeeded).Select(r => r.Key))}");
            }
        }

        public async Task<IEnumerable<AzureBatchRemoveResultDto>> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models)
        {
            if (!models.HasAny())
            {
                return Enumerable.Empty<AzureBatchRemoveResultDto>();
            }

            var batchActions = models
                .Select(model => new IndexAction<TAzureModel>(model, IndexActionType.Delete))
                .ToList();

            var batch = IndexBatch.New(batchActions);
            DocumentIndexResult result = null;

            try
            {
                result = await _searchIndex.Documents.IndexAsync(batch);
            }
            catch (IndexBatchException ex)
            {
                //                _logger.Error<AzureIndexer>(ex,
                //                    $"Failed to drop some documents: {string.Join(", ", ex.IndexingResults.Where(x => !x.Succeeded).Select(r => r.Key))}");
            }

            return result?.Results
                       .Select(r => new AzureBatchRemoveResultDto
                       {
                           Succeeded = r.Succeeded,
                           ModelId = r.Key
                       })
                       .ToList()
                   ?? Enumerable.Empty<AzureBatchRemoveResultDto>();
        }
    }
}