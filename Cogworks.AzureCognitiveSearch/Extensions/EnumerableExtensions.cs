using System.Collections.Generic;
using System.Linq;

namespace Cogworks.AzureCognitiveSearch.Extensions
{
    internal static class EnumerableExtensions
    {
        public static bool HasAny<T>(this IEnumerable<T> items)
            => items != null && items.Any();

        public static IEnumerable<IEnumerable<T>> ChunkBy<T>(this IEnumerable<T> items, int chunkSize)
            => items
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
    }
}