using Newtonsoft.Json;

namespace Cogworks.AzureSearch.Models
{
    public interface IAzureIndexModel : IAzureModelIdentity
    {
        [JsonIgnore]
        string IndexName { get; }
    }
}