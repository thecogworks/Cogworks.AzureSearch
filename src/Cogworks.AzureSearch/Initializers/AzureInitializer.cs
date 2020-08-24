using Cogworks.AzureSearch.Interfaces;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using Cogworks.AzureSearch.Options;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Initializers
{
    public interface IAzureInitializer<in TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        Task<AzureIndexOperationResult> InitializeAsync();
    }

    public class AzureInitializer<TAzureModel> : IAzureInitializer<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
        private readonly AzureSearchIndexOption _azureSearchIndexOption;
        private readonly IAzureIndexOperation<TAzureModel> _azureIndexOperation;

        public AzureInitializer(AzureSearchIndexOption azureSearchIndexOption, IAzureIndexOperation<TAzureModel> azureIndexOperation)
        {
            _azureSearchIndexOption = azureSearchIndexOption;
            _azureIndexOperation = azureIndexOperation;
        }

        public async Task<AzureIndexOperationResult> InitializeAsync()
        {
            if (_azureSearchIndexOption.Recreate)
            {
                await _azureIndexOperation.IndexDeleteAsync();
            }

            return await _azureIndexOperation.IndexCreateOrUpdateAsync();
        }
    }
}