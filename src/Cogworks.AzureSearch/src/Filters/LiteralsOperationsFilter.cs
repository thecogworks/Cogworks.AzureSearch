using Cogworks.AzureSearch.Extensions;

namespace Cogworks.AzureSearch.Filters
{
    public static class LiteralsOperationsFilter
    {
        public static string Field(this string query, string field)
            => query.HasValue()
                ? $"{query} {field}"
                : field;

        public static string Value(this string query, string value)
            => $"{query} '{value}'";

        public static string Property(this string query, string value)
            => $"{query}/{value}";

        public static string Append(this string query, string value)
            => value.HasValue() ? $"{query} {value}" : query;
    }
}