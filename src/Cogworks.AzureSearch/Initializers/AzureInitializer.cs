using System;
using System.Threading.Tasks;
using Cogworks.AzureSearch.Exceptions.IndexExceptions;
using Cogworks.AzureSearch.Interfaces.Initializers;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;

namespace Cogworks.AzureSearch.Initializers
{
    internal class AzureInitializer<TAzureModel> : IAzureInitializer<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
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

            try
            {
                await _azureIndexOperation.IndexCreateOrUpdateAsync();
            }
            catch (Exception)
            {
                if (_azureSearchIndexOption.RecreateOnUpdateFailure)
                {
                    await _azureIndexOperation.IndexDeleteAsync();
                }
            }

            try
            {
                await _azureIndexOperation.IndexCreateOrUpdateAsync();
            }
            catch (Exception exception)
            {
                throw new IndexInitializerException(exception.Message, exception.InnerException);
            }
        }
    }
}