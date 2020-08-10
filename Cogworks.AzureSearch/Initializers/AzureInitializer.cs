using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;
using Cogworks.AzureSearch.Repositories;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Initializers
{
    public interface IAzureInitializer<in TAzureModel> where TAzureModel : IAzureModelIdentity
    {
        Task InitializeAsync();
    }

    public class AzureInitializer<TAzureModel> : IAzureInitializer<TAzureModel>
        where TAzureModel : IAzureModelIdentity
    {
        private readonly AzureSearchIndexOption _azureSearchIndexOption;
        private readonly IAzureIndexOperation<TAzureModel> _azureIndexOperation;

        public AzureInitializer(AzureSearchIndexOption azureSearchIndexOption, IAzureIndexOperation<TAzureModel> azureIndexOperation)
        {
            _azureSearchIndexOption = azureSearchIndexOption;
            _azureIndexOperation = azureIndexOperation;
        }

        public async Task InitializeAsync()
        {
            if (_azureSearchIndexOption.Recreate)
            {
                await _azureIndexOperation.IndexDeleteAsync();
            }

            // todo: catch exception or dto and validate with options
            await _azureIndexOperation.IndexCreateOrUpdateAsync();
        }
    }
}