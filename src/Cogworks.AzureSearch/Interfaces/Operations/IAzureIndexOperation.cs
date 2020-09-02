using System.Threading.Tasks;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Interfaces.Operations
{
    public interface IAzureIndexOperation<in TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        Task<bool> IndexExistsAsync();

        Task<AzureIndexOperationResult> IndexDeleteAsync();

        Task<AzureIndexOperationResult> IndexCreateOrUpdateAsync();

        Task<AzureIndexOperationResult> IndexClearAsync();
    }
}