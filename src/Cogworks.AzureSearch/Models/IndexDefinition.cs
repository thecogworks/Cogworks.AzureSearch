using Azure.Search.Documents.Indexes.Models;

namespace Cogworks.AzureSearch.Models
{
    public class IndexDefinition<TAzureModel> where TAzureModel : class, IModel, new()
    {
        public string IndexName { get; }

        public SearchIndex CustomIndexDefinition { get; }

        public IndexDefinition(string indexName)
            => IndexName = indexName;

        public IndexDefinition(SearchIndex customIndex)
            => (CustomIndexDefinition, IndexName) = (customIndex, customIndex.Name);
    }
}