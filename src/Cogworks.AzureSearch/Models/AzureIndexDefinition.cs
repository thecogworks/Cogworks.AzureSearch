namespace Cogworks.AzureSearch.Models
{
    public class AzureIndexDefinition<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        public string IndexName { get; }

        public AzureIndexDefinition(string indexName)
            => IndexName = indexName;
    }
}