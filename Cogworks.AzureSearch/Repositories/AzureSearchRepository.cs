﻿using Cogworks.AzureSearch.Extensions;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using Cogworks.AzureSearch.Options;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Repositories
{
    public interface IAzureIndexOperation<in TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        Task<bool> IndexExistsAsync();

        Task<AzureIndexOperationResultDto> IndexDeleteAsync();

        Task<AzureIndexOperationResultDto> IndexCreateOrUpdateAsync();

        Task<AzureIndexOperationResultDto> IndexClearAsync();
    }

    public interface IAzureDocumentOperation<in TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        Task<AzureDocumentOperationResultDto> AddOrUpdateDocumentAsync(TAzureModel model);

        Task<AzureBatchDocumentsOperationResultDto> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models);

        Task<AzureDocumentOperationResultDto> TryRemoveDocumentAsync(TAzureModel model);

        Task<AzureBatchDocumentsOperationResultDto> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models);
    }

    public interface IAzureSearchRepository<in TAzureModel> : IAzureDocumentOperation<TAzureModel>, IAzureIndexOperation<TAzureModel>
        where TAzureModel : IAzureModelIdentity
    {
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

        public async Task<AzureIndexOperationResultDto> IndexDeleteAsync()
        {
            var result = new AzureIndexOperationResultDto();

            try
            {
                await _searchServiceClient.Indexes.DeleteAsync(_azureIndexDefinition.IndexName);

                result.Succeeded = true;
                result.Message = $"Index {_azureIndexDefinition.IndexName} successfully deleted.";
            }
            catch (Exception e)
            {
                result.Message = $"An issue occured on deleting index: {_azureIndexDefinition.IndexName}. More information: {e.Message}";
            }

            return result;
        }

        public async Task<AzureIndexOperationResultDto> IndexCreateOrUpdateAsync()
        {
            var indexDefinition = new Index()
            {
                Name = _azureIndexDefinition.IndexName,
                Fields = FieldBuilder.BuildForType<TAzureModel>()
            };

            var result = new AzureIndexOperationResultDto();

            try
            {
                await _searchServiceClient.Indexes.CreateOrUpdateAsync(indexDefinition);

                result.Message = $"Index {_azureIndexDefinition.IndexName} successfully created or updated.";
                result.Succeeded = true;
            }
            catch (Exception exception)
            {
                result.Message = $"An issue occured on creating or updating index: {_azureIndexDefinition.IndexName}. More information: {exception.Message}";
            }

            return result;
        }

        public async Task<AzureIndexOperationResultDto> IndexClearAsync()
        {
            if (await IndexExistsAsync())
            {
                await IndexDeleteAsync();
            }

            return await IndexCreateOrUpdateAsync();
        }

        public async Task<AzureDocumentOperationResultDto> AddOrUpdateDocumentAsync(TAzureModel model)
        {
            var azureBatchDocumentsOperationResult = await AddOrUpdateDocumentsAsync(new[] { model });

            return azureBatchDocumentsOperationResult.Succeeded
                ? azureBatchDocumentsOperationResult.SucceededDocuments.FirstOrDefault()
                : azureBatchDocumentsOperationResult.FailedDocuments.FirstOrDefault();
        }

        public async Task<AzureBatchDocumentsOperationResultDto> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models)
        {
            if (!models.HasAny())
            {
                return new AzureBatchDocumentsOperationResultDto()
                {
                    Succeeded = true,
                    Message = "No documents found to index."
                };
            }

            var batchActions = models
                .Select(model => new IndexAction<TAzureModel>(model, IndexActionType.Upload))
                .ToList();

            var batch = IndexBatch.New(batchActions);

            var indexResults = Enumerable.Empty<IndexingResult>();

            try
            {
                var result = await _searchIndex.Documents.IndexAsync(batch);
                indexResults = result.Results;
            }
            catch (IndexBatchException indexBatchException)
            {
                indexResults = indexBatchException.IndexingResults;
            }
            catch (Exception exception)
            {
                // todo: handle it proper
            }

            return GetBatchOperationStatus(indexResults, "adding or updating");
        }

        public async Task<AzureDocumentOperationResultDto> TryRemoveDocumentAsync(TAzureModel model)
        {
            var azureBatchDocumentsOperationResult = await TryRemoveDocumentsAsync(new[] { model });

            return azureBatchDocumentsOperationResult.Succeeded
                ? azureBatchDocumentsOperationResult.SucceededDocuments.FirstOrDefault()
                : azureBatchDocumentsOperationResult.FailedDocuments.FirstOrDefault();
        }

        public async Task<AzureBatchDocumentsOperationResultDto> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models)
        {
            if (!models.HasAny())
            {
                return new AzureBatchDocumentsOperationResultDto()
                {
                    Succeeded = true,
                    Message = "No documents found to delete."
                };
            }

            var batchActions = models
                .Select(model => new IndexAction<TAzureModel>(model, IndexActionType.Delete))
                .ToList();

            var batch = IndexBatch.New(batchActions);
            var indexResults = Enumerable.Empty<IndexingResult>();

            try
            {
                var result = await _searchIndex.Documents.IndexAsync(batch);
                indexResults = result.Results;
            }
            catch (IndexBatchException indexBatchException)
            {
                indexResults = indexBatchException.IndexingResults;
            }
            catch (Exception exception)
            {
                // todo: handle it proper
            }

            return GetBatchOperationStatus(indexResults, "removing");
        }

        private AzureBatchDocumentsOperationResultDto GetBatchOperationStatus(IEnumerable<IndexingResult> indexingResults, string operationType)
        {
            var successfullyItems = indexingResults.Where(x => x.Succeeded).ToList();
            var failedItems = indexingResults.Where(x => !x.Succeeded).ToList();

            return new AzureBatchDocumentsOperationResultDto
            {
                Succeeded = !failedItems.Any(),
                SucceededDocuments = successfullyItems.Select(successfullyItem => new AzureDocumentOperationResultDto
                {
                    Succeeded = true,
                    Message = $"Successfully {operationType} document.",
                    ModelId = successfullyItem.Key,
                    StatusCode = successfullyItem.StatusCode
                }).ToList(),
                FailedDocuments = failedItems.Select(failedItem => new AzureDocumentOperationResultDto
                {
                    Message = $"Failed {operationType} document.",
                    ModelId = failedItem.Key,
                    StatusCode = failedItem.StatusCode
                }).ToList(),
            };
        }
    }
}