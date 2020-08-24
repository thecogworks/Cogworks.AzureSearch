using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Interfaces.Initializers
{
    public interface IAzureInitializer<in TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        Task<AzureIndexOperationResult> InitializeAsync();
    }
}