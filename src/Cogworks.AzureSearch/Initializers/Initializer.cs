using System;
using System.Threading.Tasks;
using Cogworks.AzureSearch.Exceptions.IndexExceptions;
using Cogworks.AzureSearch.Interfaces.Initializers;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;

namespace Cogworks.AzureSearch.Initializers
{
    internal class Initializer<TAzureModel> : IInitializer<TAzureModel>
        where TAzureModel : class, IModel, new()
    {
        private readonly AzureSearchIndexOption _azureSearchIndexOption;
        private readonly IIndexOperation<TAzureModel> _indexOperation;

        public Initializer(AzureSearchIndexOption azureSearchIndexOption, IIndexOperation<TAzureModel> indexOperation)
        {
            _azureSearchIndexOption = azureSearchIndexOption;
            _indexOperation = indexOperation;
        }

        public async Task InitializeAsync()
        {
            if (_azureSearchIndexOption.Recreate)
            {
                await _indexOperation.IndexDeleteAsync();
            }

            try
            {
                await _indexOperation.IndexCreateOrUpdateAsync();
            }
            catch (Exception)
            {
                if (_azureSearchIndexOption.RecreateOnUpdateFailure)
                {
                    await _indexOperation.IndexDeleteAsync();
                }
            }

            try
            {
                await _indexOperation.IndexCreateOrUpdateAsync();
            }
            catch (Exception exception)
            {
                throw new IndexInitializerException(exception.Message, exception.InnerException);
            }
        }
    }
}