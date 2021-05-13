using System;
using System.Threading.Tasks;
using Cogworks.AzureSearch.Exceptions.IndexExceptions;
using Cogworks.AzureSearch.Interfaces.Initializers;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;

namespace Cogworks.AzureSearch.Initializers
{
    internal class Initializer<TModel> : IInitializer<TModel>
        where TModel : class, IModel, new()
    {
        private readonly IndexOption _indexOption;
        private readonly IIndexOperation<TModel> _indexOperation;

        public Initializer(IndexOption indexOption, IIndexOperation<TModel> indexOperation)
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