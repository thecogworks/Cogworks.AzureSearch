using Azure.Search.Documents.Indexes.Models;

namespace Cogworks.AzureSearch.Models
{
    public class AzureIndexDefinition<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        public string IndexName { get; }

        public SearchIndex CustomIndexDefinition { get; }

        public AzureIndexDefinition(string indexName)
            => IndexName = indexName;

        public AzureIndexDefinition(SearchIndex customIndex)
            => (CustomIndexDefinition, IndexName) = (customIndex, customIndex.Name);
    }
}