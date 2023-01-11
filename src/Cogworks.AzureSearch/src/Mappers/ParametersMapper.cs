using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Cogworks.AzureSearch.Enums;
using Cogworks.AzureSearch.Extensions;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Mappers
{
    public static class ParametersMapper
    {
        internal static SearchOptions Map(SearchParameters searchParameters)
        {
            var searchOptions = new SearchOptions
            {
                Filter = searchParameters.Filter,
                HighlightPostTag = searchParameters.HighlightPostTag,
                HighlightPreTag = searchParameters.HighlightPreTag,
                IncludeTotalCount = searchParameters.IncludeTotalResultCount,
                MinimumCoverage = searchParameters.MinimumCoverage,
                QueryType = searchParameters.QueryType == QueryType.Full
                    ? SearchQueryType.Full
                    : SearchQueryType.Simple,
                ScoringProfile = searchParameters.ScoringProfile,
                SearchMode = searchParameters.SearchMode == SearchModeType.Any
                    ? SearchMode.Any
                    : SearchMode.All,
                Skip = searchParameters.Skip,
                Size = searchParameters.Take
            };

            if (searchParameters.Select.HasAny())
            {
                foreach (var selectField in searchParameters.Select)
                {
                    searchOptions.Select.Add(selectField);
                }
            }

            if (searchParameters.SearchFields.HasAny())
            {
                foreach (var searchField in searchParameters.SearchFields)
                {
                    searchOptions.SearchFields.Add(searchField);
                }
            }

            if (searchParameters.HighlightFields.HasAny())
            {
                foreach (var highlightField in searchParameters.HighlightFields)
                {
                    searchOptions.HighlightFields.Add(highlightField);
                }
            }

            if (searchParameters.Facets.HasAny())
            {
                foreach (var facet in searchParameters.Facets)
                {
                    searchOptions.Facets.Add(facet);
                }
            }

            if (searchParameters.OrderBy.HasAny())
            {
                foreach (var order in searchParameters.OrderBy)
                {
                    searchOptions.OrderBy.Add(order);
                }
            }

            return searchOptions;
        }
    }
}