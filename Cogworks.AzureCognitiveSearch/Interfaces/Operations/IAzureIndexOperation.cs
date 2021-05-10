using System.Threading.Tasks;
using Cogworks.AzureCognitiveSearch.Models;
using Cogworks.AzureCognitiveSearch.Models.Dtos;

namespace Cogworks.AzureCognitiveSearch.Interfaces.Operations
{
    public interface IAzureIndexOperation<in TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        Task<bool> IndexExistsAsync();

        Task IndexDeleteAsync();

        Task IndexCreateOrUpdateAsync();

        Task IndexClearAsync();
    }
}