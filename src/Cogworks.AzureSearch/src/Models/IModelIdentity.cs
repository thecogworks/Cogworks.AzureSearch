namespace Cogworks.AzureSearch.Models
{
    public interface IModelIdentity : IModel
    {
        string Id { get; set; }
    }
}