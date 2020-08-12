using Cogworks.AzureSearch.Extensions;
using System.Collections.Generic;

namespace Cogworks.AzureSearch.Filters
{
    public static class LogicOperationsFilter
    {
        public static string And(this string query)
            => query.HasValue() && !query.EndsWith(" and")
                ? $"{query} and"
                : query;

        public static string Or(this string query)
            => query.HasValue() && !query.EndsWith(" or") ? $"{query} or" : query;

        public static string Not(this string query)
            => query.HasValue() && !query.EndsWith(" not") ? $"{query} not" : query;

        public static string Equals(this string query)
            => query.HasValue() && !query.EndsWith(" eq") ? $"{query} eq" : query;

        public static string EqualsValue(this string query, string value)
        {
            var equalsQuery = query.Equals();

            return equalsQuery.HasValue() && equalsQuery.EndsWith(" eq") && value.HasValue()
                ? equalsQuery.Value(value)
                : query;
        }

        public static string EqualsNull(this string query)
            => query.HasValue() && !query.EndsWith($" {Constants.StringConstants.Terms.Null}")
                ? $"{query.Equals()} {Constants.StringConstants.Terms.Null}"
                : query;

        public static string NotEquals(this string query)
            => query.HasValue() && !query.EndsWith(" ne") ? $"{query} ne" : query;

        public static string NotEqualsValue(this string query, string value)
        {
            var notEqualsQuery = query.NotEquals();

            return notEqualsQuery.HasValue() && notEqualsQuery.EndsWith(" ne") && value.HasValue()
                ? notEqualsQuery.Value(value)
                : query;
        }

        public static string NotEqualsNull(this string query)
            => query.HasValue() && !query.EndsWith(" ne")
                ? $"{query.NotEquals()} {Constants.StringConstants.Terms.Null}"
                : query;

        public static string Any(this string query, string collectionName, string condition)
        {
            if (collectionName.HasValue() && condition.HasValue())
            {
                return query.HasValue()
                    ? $"{query} {collectionName}/any({condition})"
                    : $"{collectionName}/any({condition})";
            }

            return query;
        }

        public static string NormalizeQuery(this string query)
        {
            var normalizedQuery = query.Trim();

            foreach (var logicalOperator in LogicalOperators())
            {
                normalizedQuery = normalizedQuery
                    .TrimEnd(logicalOperator.ToCharArray())
                    .TrimStart(logicalOperator.ToCharArray());
            }

            return normalizedQuery;
        }

        private static IEnumerable<string> LogicalOperators()
            => new[]
            {
                "and",
                "or",
                "ne",
                "eq",
                "not"
            };
    }
}