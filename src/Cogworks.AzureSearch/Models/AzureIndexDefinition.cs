using System.Runtime.CompilerServices;
using Microsoft.Azure.Search.Models;

namespace Cogworks.AzureSearch.Models
{
    public class AzureIndexDefinition<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        public string IndexName { get; }

        public Index CustomIndexDefinition { get; }

        public AzureIndexDefinition(string indexName)
            => IndexName = indexName;

        public AzureIndexDefinition(Index customIndex)
            => (CustomIndexDefinition, IndexName) = (customIndex, customIndex.Name);
    }
}