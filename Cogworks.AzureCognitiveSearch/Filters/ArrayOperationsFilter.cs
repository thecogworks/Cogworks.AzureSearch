using Cogworks.AzureCognitiveSearch.Extensions;

namespace Cogworks.AzureCognitiveSearch.Filters
{
    public static class ArrayOperationsFilter
    {
        public static string SearchIn(this string query, string field, string searchedValue, string separator = Constants.StringConstants.Separators.Comma)
        {
            if (!field.HasValue())
            {
                return query;
            }

            var searchQuery = $"search.in({field}, '{searchedValue}', '{separator}')";

            return query.HasValue()
                ? $"{query} {searchQuery}"
                : searchQuery;
        }
    }
}