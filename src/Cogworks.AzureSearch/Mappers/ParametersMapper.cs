using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Cogworks.AzureSearch.Enums;
using Cogworks.AzureSearch.Extensions;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Mappers
{
    public static class ParametersMapper
    {
        internal static SearchOptions Map(AzureSearchParameters azureSearchParameters)
        {
            var searchParameters = new SearchOptions
            {
                Filter = azureSearchParameters.Filter,
                HighlightPostTag = azureSearchParameters.HighlightPostTag,
                HighlightPreTag = azureSearchParameters.HighlightPreTag,
                IncludeTotalCount = azureSearchParameters.IncludeTotalResultCount,
                MinimumCoverage = azureSearchParameters.MinimumCoverage,
                QueryType = azureSearchParameters.QueryType == AzureQueryType.Full
                    ? SearchQueryType.Full
                    : SearchQueryType.Simple,
                ScoringProfile = azureSearchParameters.ScoringProfile,
                SearchMode = azureSearchParameters.SearchMode == AzureSearchModeType.Any
                    ? SearchMode.Any
                    : SearchMode.All,
                Skip = azureSearchParameters.Skip,
                Size = azureSearchParameters.Take
            };

            if (azureSearchParameters.Select.HasAny())
            {
                foreach (var selectField in azureSearchParameters.Select)
                {
                    searchParameters.Select.Add(selectField);
                }
            }

            if (azureSearchParameters.SearchFields.HasAny())
            {
                foreach (var searchField in azureSearchParameters.SearchFields)
                {
                    searchParameters.SearchFields.Add(searchField);
                }
            }

            if (azureSearchParameters.HighlightFields.HasAny())
            {
                foreach (var highlightField in azureSearchParameters.HighlightFields)
                {
                    searchParameters.HighlightFields.Add(highlightField);
                }
            }

            if (azureSearchParameters.Facets.HasAny())
            {
                foreach (var facet in azureSearchParameters.Facets)
                {
                    searchParameters.Facets.Add(facet);
                }
            }

            if (azureSearchParameters.OrderBy.HasAny())
            {
                foreach (var order in azureSearchParameters.OrderBy)
                {
                    searchParameters.OrderBy.Add(order);
                }
            }

            return searchParameters;
        }
    }
}