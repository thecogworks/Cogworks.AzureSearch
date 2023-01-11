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
    public class DocumentOperation<TModel> : IDocumentOperation<TModel>
        where TModel : class, IModel, new()
    {
        private readonly IDocumentOperationWrapper<TModel> _documentOperationWrapper;

        private const int BatchOperationSize = 500;

        public DocumentOperation(IDocumentOperationWrapper<TModel> documentOperationWrapper)
            => _documentOperationWrapper = documentOperationWrapper;

        public async Task<DocumentOperationResult> AddOrUpdateDocumentAsync(TModel model)
        {
            var azureBatchDocumentsOperationResult = await AddOrUpdateDocumentsAsync(new[] { model });

            return azureBatchDocumentsOperationResult.Succeeded
                ? azureBatchDocumentsOperationResult.SucceededDocuments.FirstOrDefault()
                : azureBatchDocumentsOperationResult.FailedDocuments.FirstOrDefault();
        }

        public async Task<BatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TModel> models)
        {
            if (!models.HasAny())
            {
                return new BatchDocumentsOperationResult()
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
                var batch = IndexDocumentsBatch.Create<TModel>(batchActions.ToArray());

                try
                {
                    var result = await _documentOperationWrapper.IndexAsync(batch);
                    indexResults.AddRange(result.Value.Results);
                }
                catch (Exception exception)
                {
                    var domainException = new AddOrUpdateDocumentException(
                        exception.Message,
                        exception.InnerException);

                    throw domainException;
                }
            }

            return GetBatchOperationStatus(indexResults, "adding or updating");
        }

        public async Task<DocumentOperationResult> TryRemoveDocumentAsync(TModel model)
        {
            var azureBatchDocumentsOperationResult = await TryRemoveDocumentsAsync(new[] { model });

            return azureBatchDocumentsOperationResult.Succeeded
                ? azureBatchDocumentsOperationResult.SucceededDocuments.FirstOrDefault()
                : azureBatchDocumentsOperationResult.FailedDocuments.FirstOrDefault();
        }

        public async Task<BatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TModel> models)
        {
            if (!models.HasAny())
            {
                return new BatchDocumentsOperationResult()
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
                var batch = IndexDocumentsBatch.Create<TModel>(batchActions.ToArray());

                try
                {
                    var result = await _documentOperationWrapper.IndexAsync(batch);
                    indexResults.AddRange(result.Value.Results);
                }
                catch (Exception exception)
                {
                    var domainException = new RemoveDocumentException(exception.Message, exception.InnerException);
                    throw domainException;
                }
            }

            return GetBatchOperationStatus(indexResults, "removing");
        }

        private static BatchDocumentsOperationResult GetBatchOperationStatus(IEnumerable<IndexingResult> indexingResults, string operationType)
        {
            var successfullyItems = indexingResults.Where(x => x.Succeeded).ToList();
            var failedItems = indexingResults.Where(x => !x.Succeeded).ToList();

            return new BatchDocumentsOperationResult
            {
                Succeeded = !failedItems.Any(),
                SucceededDocuments = successfullyItems.Select(successfullyItem => new DocumentOperationResult
                {
                    Succeeded = true,
                    Message = $"Successfully {operationType} document.",
                    ModelId = successfullyItem.Key,
                    StatusCode = successfullyItem.Status
                }).ToList(),
                FailedDocuments = failedItems.Select(failedItem => new DocumentOperationResult
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