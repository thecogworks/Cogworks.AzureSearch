using Cogworks.AzureCognitiveSearch.Extensions;

namespace Cogworks.AzureCognitiveSearch.Filters
{
    public static class RangeFilter
    {
        public static string GreaterThan(this string query, string field, string value)
            => BuildRangeFilter(query, field, value, "gt");

        public static string GreaterThanOrEqualTo(this string query, string field, string value)
            => BuildRangeFilter(query, field, value, "ge");

        public static string LessThan(this string query, string field, string value)
            => BuildRangeFilter(query, field, value, "lt");

        public static string LessThanOrEqualTo(this string query, string field, string value)
            => BuildRangeFilter(query, field, value, "le");

        private static string BuildRangeFilter(string query, string field, string value, string @operator)
        {
            if (!field.HasValue() || !value.HasValue())
            {
                return query;
            }

            var resultQuery = $"{field} {@operator} {value}";

            return query.HasValue()
                ? $"{query} {resultQuery}"
                : resultQuery;
        }
    }
}