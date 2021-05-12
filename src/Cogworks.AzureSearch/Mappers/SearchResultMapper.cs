using System.Linq;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Mappers
{
    public static class SearchResultMapper
    {
        public static SearchResult<TModel> Map<TModel>(
            Azure.Search.Documents.Models.SearchResults<TModel> results,
            int skip,
            int take) where TModel : class, IModel, new()
        {
            var resultsCount = results.TotalCount ?? 0;

            var searchedDocuments = results.GetResults()
                .Select(resultDocument => new SearchResultItem<TModel>(
                    resultDocument.Document,
                    resultDocument.Highlights,
                    resultDocument.Score ?? default
                ))
                .ToArray();

            return new SearchResult<TModel>()
            {
                HasMoreItems = skip + take < resultsCount,
                TotalCount = resultsCount,
                Results = searchedDocuments
            };
        }
    }
}