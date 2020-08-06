using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Core.Models.SearchModels
{
    public class EventDocument : IAzureModelIdentity
    {
        public string Id { get; set; }
    }
}