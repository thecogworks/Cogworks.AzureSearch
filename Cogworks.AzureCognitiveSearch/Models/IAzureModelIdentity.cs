namespace Cogworks.AzureCognitiveSearch.Models
{
    public interface IAzureModelIdentity : IAzureModel
    {
        string Id { get; set; }
    }
}