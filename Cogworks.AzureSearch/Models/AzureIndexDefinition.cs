namespace Cogworks.AzureSearch.Models
{
    public class AzureIndexDefinition<TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        public string IndexName { get; }

        public AzureIndexDefinition(string indexName)
            => IndexName = indexName;
    }
}