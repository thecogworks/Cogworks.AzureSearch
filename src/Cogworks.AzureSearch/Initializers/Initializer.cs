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
        private readonly IndexOption _indexOption;
        private readonly IIndexOperation<TAzureModel> _indexOperation;

        public Initializer(IndexOption indexOption, IIndexOperation<TAzureModel> indexOperation)
        {
            _indexOption = indexOption;
            _indexOperation = indexOperation;
        }

        public async Task InitializeAsync()
        {
            if (_indexOption.Recreate)
            {
                await _indexOperation.IndexDeleteAsync();
            }

            try
            {
                await _indexOperation.IndexCreateOrUpdateAsync();
            }
            catch (Exception)
            {
                if (_indexOption.RecreateOnUpdateFailure)
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