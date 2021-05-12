using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Search.Documents.Models;
using Cogworks.AzureSearch.Exceptions.DocumentsExceptions;
using Cogworks.AzureSearch.Extensions;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Operations
{
    public class AzureDocumentOperation<TAzureModel> : IAzureDocumentOperation<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
        private readonly IDocumentOperationWrapper<TAzureModel> _documentOperationWrapper;

        private const int BatchOperationSize = 500;

        public AzureDocumentOperation(IDocumentOperationWrapper<TAzureModel> documentOperationWrapper)
            => _documentOperationWrapper = documentOperationWrapper;

        public async Task<AzureDocumentOperationResult> AddOrUpdateDocumentAsync(TAzureModel model)
        {
            var azureBatchDocumentsOperationResult = await AddOrUpdateDocumentsAsync(new[] { model });

            return azureBatchDocumentsOperationResult.Succeeded
                ? azureBatchDocumentsOperationResult.SucceededDocuments.FirstOrDefault()
                : azureBatchDocumentsOperationResult.FailedDocuments.FirstOrDefault();
        }

        public async Task<AzureBatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models)
        {
            if (!models.HasAny())
            {
                return new AzureBatchDocumentsOperationResult()
                {
                    Succeeded = true,
                    Message = "No documents found to index."
                };
            }

            var chunkedBatchActions = models
                .Select(IndexDocumentsAction.MergeOrUpload)
                .ChunkBy(BatchOperationSize)
                .ToList();

            var indexResults = new List<IndexingResult>();

            foreach (var batchActions in chunkedBatchActions)
            {
                var batch = IndexDocumentsBatch.Create<TAzureModel>(batchActions.ToArray());

                try
                {
                    var result = await _documentOperationWrapper.IndexAsync(batch);
                    indexResults.AddRange(result.Value.Results);
                }
                catch (Exception exception)
                {
                    throw new AddOrUpdateDocumentException(exception.Message, exception.InnerException);
                }
            }

            return GetBatchOperationStatus(indexResults, "adding or updating");
        }

        public async Task<AzureDocumentOperationResult> TryRemoveDocumentAsync(TAzureModel model)
        {
            var azureBatchDocumentsOperationResult = await TryRemoveDocumentsAsync(new[] { model });

            return azureBatchDocumentsOperationResult.Succeeded
                ? azureBatchDocumentsOperationResult.SucceededDocuments.FirstOrDefault()
                : azureBatchDocumentsOperationResult.FailedDocuments.FirstOrDefault();
        }

        public async Task<AzureBatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models)
        {
            if (!models.HasAny())
            {
                return new AzureBatchDocumentsOperationResult()
                {
                    Succeeded = true,
                    Message = "No documents found to delete."
                };
            }

            var chunkedBatchActions = models
                .Select(IndexDocumentsAction.Delete)
                .ChunkBy(BatchOperationSize)
                .ToList();

            var indexResults = new List<IndexingResult>();

            foreach (var batchActions in chunkedBatchActions)
            {
                var batch = IndexDocumentsBatch.Create<TAzureModel>(batchActions.ToArray());

                try
                {
                    var result = await _documentOperationWrapper.IndexAsync(batch);
                    indexResults.AddRange(result.Value.Results);
                }
                catch (Exception exception)
                {
                    throw new RemoveDocumentException(exception.Message, exception.InnerException);
                }
            }

            return GetBatchOperationStatus(indexResults, "removing");
        }

        private static AzureBatchDocumentsOperationResult GetBatchOperationStatus(IEnumerable<IndexingResult> indexingResults, string operationType)
        {
            var successfullyItems = indexingResults.Where(x => x.Succeeded).ToList();
            var failedItems = indexingResults.Where(x => !x.Succeeded).ToList();

            return new AzureBatchDocumentsOperationResult
            {
                Succeeded = !failedItems.Any(),
                SucceededDocuments = successfullyItems.Select(successfullyItem => new AzureDocumentOperationResult
                {
                    Succeeded = true,
                    Message = $"Successfully {operationType} document.",
                    ModelId = successfullyItem.Key,
                    StatusCode = successfullyItem.Status
                }).ToList(),
                FailedDocuments = failedItems.Select(failedItem => new AzureDocumentOperationResult
                {
                    Message = $"Failed {operationType} document.",
                    InnerMessage = failedItem.ErrorMessage,
                    ModelId = failedItem.Key,
                    StatusCode = failedItem.Status
                }).ToList()
            };
        }
    }
}