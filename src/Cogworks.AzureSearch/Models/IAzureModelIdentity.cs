namespace Cogworks.AzureSearch.Models
{
    public interface IAzureModelIdentity : IAzureModel
    {
        string Id { get; set; }
    }
}