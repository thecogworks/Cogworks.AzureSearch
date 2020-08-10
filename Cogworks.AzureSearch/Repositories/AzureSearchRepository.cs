using Cogworks.AzureSearch.Extensions;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using Cogworks.AzureSearch.Options;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Repositories
{
    public interface IAzureSearchRepository<TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        // TODO: move it to Initializer

        Task<bool> IndexExistsAsync();

        Task IndexDeleteAsync();

        Task IndexCreateOrUpdateAsync();

        Task IndexClearAsync();

        Task AddOrUpdateDocumentAsync(TAzureModel model);

        Task AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models);

        Task<AzureRemoveResultDto> TryRemoveDocumentAsync(TAzureModel model);

        Task<IEnumerable<AzureRemoveResultDto>> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models);

        // TODO add search options
    }

    public class AzureSearchRepository<TAzureModel> : IAzureSearchRepository<TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        private readonly AzureIndexDefinition<TAzureModel> _azureIndexDefinition;
        private readonly ISearchIndexClient _searchIndex;
        private readonly ISearchServiceClient _searchServiceClient;

        public AzureSearchRepository(AzureIndexDefinition<TAzureModel> azureIndexDefinition, AzureSearchClientOption azureSearchClientOption)
        {
            _azureIndexDefinition = azureIndexDefinition;
            _searchServiceClient = azureSearchClientOption.GetSearchServiceClient();
            _searchIndex = _searchServiceClient.Indexes.GetClient(azureIndexDefinition.IndexName);
        }

        public Task<bool> IndexExistsAsync()
            => _searchServiceClient.Indexes.ExistsAsync(_azureIndexDefinition.IndexName);

        public async Task IndexDeleteAsync()
        {
            try
            {
                await _searchServiceClient.Indexes.DeleteAsync(_azureIndexDefinition.IndexName);
            }
            catch (Exception e)
            {
            }
        }

        // TODO: result DTO
        public async Task IndexCreateOrUpdateAsync()
        {
            Index indexDefinition = new Index()
            {
                Name = _azureIndexDefinition.IndexName,
                Fields = FieldBuilder.BuildForType<TAzureModel>()
            };

            try
            {
                await _searchServiceClient.Indexes.CreateOrUpdateAsync(indexDefinition);
            }
            catch (CloudException cloudException)
            {
                //                _logger.Error(typeof(CloudException), cloudException, cloudException.Message);
            }
            catch (SerializationException serializationException)
            {
                //                _logger.Error(typeof(SerializationException), serializationException, serializationException.Message);
            }
            catch (ValidationException validationException)
            {
                //                _logger.Error(typeof(ValidationException), validationException, validationException.Message);
            }
            catch (ArgumentException argumentException)
            {
                //                _logger.Error(typeof(ArgumentException), argumentException, argumentException.Message);
            }
            catch (Exception exception)
            {
                //                _logger.Error(typeof(Exception), exception, exception.Message);
            }
        }

        public async Task IndexClearAsync()
        {
            if (await IndexExistsAsync())
            {
                await IndexDeleteAsync();
            }

            await IndexCreateOrUpdateAsync();
        }

        public async Task AddOrUpdateDocumentAsync(TAzureModel model)
            => await AddOrUpdateDocumentsAsync(new[] { model });

        // TODO: DTO as result, in libraries we cannot have loggers !!!
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
                //                _logger.Error<AzureIndex>(ex,
                //                    $"Failed to index some documents: {string.Join(", ", ex.IndexingResults.Where(x => !x.Succeeded).Select(r => r.Key))}");
            }
        }

        public async Task<AzureRemoveResultDto> TryRemoveDocumentAsync(TAzureModel model)
            => (await TryRemoveDocumentsAsync(new[] { model })).FirstOrDefault();

        public async Task<IEnumerable<AzureRemoveResultDto>> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models)
        {
            if (!models.HasAny())
            {
                return Enumerable.Empty<AzureRemoveResultDto>();
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
                //                _logger.Error<AzureIndex>(ex,
                //                    $"Failed to drop some documents: {string.Join(", ", ex.IndexingResults.Where(x => !x.Succeeded).Select(r => r.Key))}");
            }

            return result?.Results
                       .Select(r => new AzureRemoveResultDto
                       {
                           Succeeded = r.Succeeded,
                           ModelId = r.Key
                       })
                       .ToList()
                   ?? Enumerable.Empty<AzureRemoveResultDto>();
        }
    }
}