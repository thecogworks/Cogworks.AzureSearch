using Cogworks.AzureSearch.Extensions;
using Cogworks.AzureSearch.Interfaces.Repositories;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using Microsoft.Azure.Search;
using Microsoft.Azure.Search.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Repositories
{
    internal class AzureSearchRepository<TAzureModel> : IAzureSearchRepository<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
        private readonly AzureIndexDefinition<TAzureModel> _azureIndexDefinition;
        private readonly IIndexOperationWrapper _indexOperationWrapper;
        private readonly IDocumentOperationWrapper<TAzureModel> _documentOperationWrapper;

        private const int BatchOperationSize = 500;

        public AzureSearchRepository(
            AzureIndexDefinition<TAzureModel> azureIndexDefinition,
            IIndexOperationWrapper indexOperationWrapper,
            IDocumentOperationWrapper<TAzureModel> documentOperationWrapper)
        {
            _azureIndexDefinition = azureIndexDefinition;
            _indexOperationWrapper = indexOperationWrapper;
            _documentOperationWrapper = documentOperationWrapper;
        }

        public async Task<bool> IndexExistsAsync()
            => await _indexOperationWrapper.ExistsAsync(_azureIndexDefinition.IndexName);

        public async Task<AzureIndexOperationResult> IndexDeleteAsync()
        {
            var result = new AzureIndexOperationResult();

            try
            {
                await _indexOperationWrapper.DeleteAsync(_azureIndexDefinition.IndexName);

                result.Succeeded = true;
                result.Message = $"Index {_azureIndexDefinition.IndexName} successfully deleted.";
            }
            catch (Exception e)
            {
                result.Message = $"An issue occurred on deleting index: {_azureIndexDefinition.IndexName}. More information: {e.Message}";
            }

            return result;
        }

        // TODO: add throwing domain exception on exception instead of returning dto
        public async Task<AzureIndexOperationResult> IndexCreateOrUpdateAsync()
        {
            var result = new AzureIndexOperationResult();

            try
            {

                var createdIndex = _azureIndexDefinition.CustomIndexDefinition != null ? 
                    await _indexOperationWrapper.CreateOrUpdateAsync<TAzureModel>(_azureIndexDefinition.CustomIndexDefinition, true) 
                    : await _indexOperationWrapper.CreateOrUpdateAsync<TAzureModel>(_azureIndexDefinition.IndexName) ;

                result.Message = $"Index {_azureIndexDefinition.IndexName} successfully created or updated.";
                result.Succeeded = true;
            }
            catch (Exception exception)
            {
                result.Message = $"An issue occurred on creating or updating index: {_azureIndexDefinition.IndexName}. More information: {exception.Message}";
            }

            return result;
        }

        public async Task<AzureIndexOperationResult> IndexCreateOrUpdateAsync(Index customIndex, bool overrideWithModelFields = true)
        {
            var result = new AzureIndexOperationResult();

            try
            {
                _ = await _indexOperationWrapper.CreateOrUpdateAsync<TAzureModel>(_azureIndexDefinition.IndexName);

                result.Message = $"Index {_azureIndexDefinition.IndexName} successfully created or updated.";
                result.Succeeded = true;
            }
            catch (Exception exception)
            {
                result.Message = $"An issue occurred on creating or updating index: {_azureIndexDefinition.IndexName}. More information: {exception.Message}";
            }

            return result;
        }

        public async Task<AzureIndexOperationResult> IndexClearAsync()
        {
            try
            {
                if (await IndexExistsAsync())
                {
                    var deleteOperationResult = await IndexDeleteAsync();

                    if (!deleteOperationResult.Succeeded)
                    {
                        return new AzureIndexOperationResult
                        {
                            Succeeded = false,
                            Message = $"An issue occurred on clearing index: {_azureIndexDefinition.IndexName}. Could not delete existing index."
                        };
                    }
                }
            }
            catch (Exception)
            {
                // ignore
            }

            var indexCreateOrUpdateResult = await IndexCreateOrUpdateAsync();

            return new AzureIndexOperationResult
            {
                Succeeded = indexCreateOrUpdateResult.Succeeded,
                Message = indexCreateOrUpdateResult.Succeeded
                    ? $"Index {_azureIndexDefinition.IndexName} successfully cleared."
                    : $"An issue occurred on clearing index: {_azureIndexDefinition.IndexName}. Could not create index."
            };
        }

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
                .Select(model => new IndexAction<TAzureModel>(model, IndexActionType.Upload))
                .ChunkBy(BatchOperationSize)
                .ToList();

            var indexResults = new List<IndexingResult>();

            foreach (var batchActions in chunkedBatchActions)
            {
                var batch = IndexBatch.New(batchActions);

                try
                {
                    var result = await _documentOperationWrapper.IndexAsync(batch);
                    indexResults.AddRange(result.Results);
                }
                catch (IndexBatchException indexBatchException)
                {
                    indexResults.AddRange(indexBatchException.IndexingResults);
                }
                catch (Exception exception)
                {
                    // todo: handle it proper
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
                .Select(model => new IndexAction<TAzureModel>(model, IndexActionType.Delete))
                .ChunkBy(BatchOperationSize)
                .ToList();

            var indexResults = new List<IndexingResult>();

            foreach (var batchActions in chunkedBatchActions)
            {
                var batch = IndexBatch.New(batchActions);

                try
                {
                    var result = await _documentOperationWrapper.IndexAsync(batch);
                    indexResults.AddRange(result.Results);
                }
                catch (IndexBatchException indexBatchException)
                {
                    indexResults.AddRange(indexBatchException.IndexingResults);
                }
                catch (Exception exception)
                {
                    // todo: handle it proper
                }
            }

            return GetBatchOperationStatus(indexResults, "removing");
        }

        public Models.Dtos.SearchResult<TAzureModel> Search(string keyword, AzureSearchParameters azureSearchParameters)
        {
            var searchText = GetSearchText(keyword);
            var parameters = GetSearchParameters(azureSearchParameters);
            var results = _documentOperationWrapper.Search($"{searchText}", parameters);

            return GetSearchResult(results, azureSearchParameters.Skip, azureSearchParameters.Take);
        }

        public async Task<Models.Dtos.SearchResult<TAzureModel>> SearchAsync(string keyword,
            AzureSearchParameters azureSearchParameters)
        {
            var searchText = GetSearchText(keyword);
            var parameters = GetSearchParameters(azureSearchParameters);
            var results = await _documentOperationWrapper.SearchAsync($"{searchText}", parameters);

            return GetSearchResult(results, azureSearchParameters.Skip, azureSearchParameters.Take);
        }

        private static string GetSearchText(string keyword)
            => keyword.EscapeHyphen().HasValue()
                ? keyword.EscapeHyphen()
                : "*";

        private static SearchParameters GetSearchParameters(AzureSearchParameters azureSearchParameters)
            => new SearchParameters
            {
                Facets = azureSearchParameters.Facets,
                Filter = azureSearchParameters.Filter,
                HighlightFields = azureSearchParameters.HighlightFields,
                HighlightPostTag = azureSearchParameters.HighlightPostTag,
                HighlightPreTag = azureSearchParameters.HighlightPreTag,
                IncludeTotalResultCount = azureSearchParameters.IncludeTotalResultCount,
                MinimumCoverage = azureSearchParameters.MinimumCoverage,
                OrderBy = azureSearchParameters.OrderBy,
                QueryType = azureSearchParameters.QueryType == Enums.AzureQueryType.Full
                    ? QueryType.Full
                    : QueryType.Simple,
                ScoringProfile = azureSearchParameters.ScoringProfile,
                SearchFields = azureSearchParameters.SearchFields,
                SearchMode = azureSearchParameters.SearchMode == Enums.AzureSearchModeType.Any
                    ? SearchMode.Any
                    : SearchMode.All,
                Select = azureSearchParameters.Select,
                Skip = azureSearchParameters.Skip,
                Top = azureSearchParameters.Take
            };

        private Models.Dtos.SearchResult<TAzureModel> GetSearchResult(DocumentSearchResult<TAzureModel> results, int skip, int take)
        {
            var resultsCount = results.Count ?? 0;

            var searchedDocuments = results.Results
                .Select(resultDocument => new SearchResultItem<TAzureModel>(
                    resultDocument.Document,
                    resultDocument.Highlights,
                    resultDocument.Score
                ))
                .ToArray();

            return new Models.Dtos.SearchResult<TAzureModel>()
            {
                HasMoreItems = skip + take < resultsCount,
                TotalCount = resultsCount,
                Results = searchedDocuments
            };
        }

        private AzureBatchDocumentsOperationResult GetBatchOperationStatus(IEnumerable<IndexingResult> indexingResults, string operationType)
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
                    StatusCode = successfullyItem.StatusCode
                }).ToList(),
                FailedDocuments = failedItems.Select(failedItem => new AzureDocumentOperationResult
                {
                    Message = $"Failed {operationType} document.",
                    ModelId = failedItem.Key,
                    StatusCode = failedItem.StatusCode
                }).ToList(),
            };
        }
    }
}