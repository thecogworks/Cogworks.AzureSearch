using System.Linq;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Mappers
{
    public static class SearchResultMapper
    {
        public static SearchResult<TAzureModel> Map<TAzureModel>(
            Azure.Search.Documents.Models.SearchResults<TAzureModel> results,
            int skip,
            int take) where TAzureModel : class, IModel, new()
        {
            var resultsCount = results.TotalCount ?? 0;

            var searchedDocuments = results.GetResults()
                .Select(resultDocument => new SearchResultItem<TAzureModel>(
                    resultDocument.Document,
                    resultDocument.Highlights,
                    resultDocument.Score ?? default
                ))
                .ToArray();

            return new SearchResult<TAzureModel>()
            {
                HasMoreItems = skip + take < resultsCount,
                TotalCount = resultsCount,
                Results = searchedDocuments
            };
        }
    }
}