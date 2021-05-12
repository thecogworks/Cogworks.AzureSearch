using System.Threading.Tasks;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Interfaces.Operations
{
    public interface IIndexOperation<in TModel> where TModel : class, IModel, new()
    {
        Task<bool> IndexExistsAsync();

        Task IndexDeleteAsync();

        Task IndexCreateOrUpdateAsync();

        Task IndexClearAsync();
    }
}