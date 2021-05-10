using Cogworks.AzureCognitiveSearch.Models;
using Cogworks.AzureCognitiveSearch.Models.Dtos;
using System.Threading.Tasks;

namespace Cogworks.AzureCognitiveSearch.Interfaces.Initializers
{
    public interface IAzureInitializer<in TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        Task<AzureIndexOperationResult> InitializeAsync();
    }
}