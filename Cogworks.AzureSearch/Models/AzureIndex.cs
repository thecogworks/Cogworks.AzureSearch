namespace Cogworks.AzureSearch.Models
{
    public class AzureIndex<TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        public string IndexName { get; }

        public AzureIndex(string indexName)
            => IndexName = indexName;
    }
}