using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Interfaces
{
    public interface IAzureIndexOperation<in TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        Task<bool> IndexExistsAsync();

        Task<AzureIndexOperationResult> IndexDeleteAsync();

        Task<AzureIndexOperationResult> IndexCreateOrUpdateAsync();

        Task<AzureIndexOperationResult> IndexClearAsync();
    }
}