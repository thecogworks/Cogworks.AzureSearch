using Cogworks.AzureSearch.Extensions;
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

        Task<AzureIndexOperationResult> IndexDeleteAsync();

        Task<AzureIndexOperationResult> IndexCreateOrUpdateAsync();

        Task<AzureIndexOperationResult> IndexClearAsync();
    }

    public interface IAzureDocumentOperation<in TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        Task<AzureDocumentOperationResult> AddOrUpdateDocumentAsync(TAzureModel model);

        Task<AzureBatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models);

        Task<AzureDocumentOperationResult> TryRemoveDocumentAsync(TAzureModel model);

        Task<AzureBatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models);
    }

    public interface IAzureDocumentSearch<TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        Models.Dtos.SearchResult<TAzureModel> Search(string keyword, AzureSearchParameters azureSearchParameters);

        Task<Models.Dtos.SearchResult<TAzureModel>> SearchAsync(string keyword, AzureSearchParameters azureSearchParameters);
    }

    public interface IAzureSearchRepository<TAzureModel> :
        IAzureDocumentOperation<TAzureModel>,
        IAzureIndexOperation<TAzureModel>,
        IAzureDocumentSearch<TAzureModel>
        where TAzureModel : IAzureModelIdentity
    {
    }

    public class AzureSearchRepository<TAzureModel> : IAzureSearchRepository<TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        private readonly AzureIndexDefinition<TAzureModel> _azureIndexDefinition;
        private readonly ISearchIndexClient _searchIndex;
        private readonly ISearchServiceClient _searchServiceClient;

        private const int BatchOperationSize = 500;

        public AzureSearchRepository(AzureIndexDefinition<TAzureModel> azureIndexDefinition, AzureSearchClientOption azureSearchClientOption)
        {
            _azureIndexDefinition = azureIndexDefinition;
            _searchServiceClient = azureSearchClientOption.GetSearchServiceClient();
            _searchIndex = _searchServiceClient.Indexes.GetClient(azureIndexDefinition.IndexName);
        }

        public Task<bool> IndexExistsAsync()
            => _searchServiceClient.Indexes.ExistsAsync(_azureIndexDefinition.IndexName);

        public async Task<AzureIndexOperationResult> IndexDeleteAsync()
        {
            var result = new AzureIndexOperationResult();

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

        public async Task<AzureIndexOperationResult> IndexCreateOrUpdateAsync()
        {
            var indexDefinition = new Index()
            {
                Name = _azureIndexDefinition.IndexName,
                Fields = FieldBuilder.BuildForType<TAzureModel>()
            };

            var result = new AzureIndexOperationResult();

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

        public async Task<AzureIndexOperationResult> IndexClearAsync()
        {
            if (await IndexExistsAsync())
            {
                await IndexDeleteAsync();
            }

            return await IndexCreateOrUpdateAsync();
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
                    var result = await _searchIndex.Documents.IndexAsync(batch);
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
                    var result = await _searchIndex.Documents.IndexAsync(batch);
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
            var results = _searchIndex.Documents.Search<TAzureModel>($"{searchText}", parameters);

            return GetSearchResult(results, azureSearchParameters.Skip, azureSearchParameters.Take);
        }

        public async Task<Models.Dtos.SearchResult<TAzureModel>> SearchAsync(string keyword,
            AzureSearchParameters azureSearchParameters)
        {
            var searchText = GetSearchText(keyword);
            var parameters = GetSearchParameters(azureSearchParameters);
            var results = await _searchIndex.Documents.SearchAsync<TAzureModel>($"{searchText}", parameters);

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