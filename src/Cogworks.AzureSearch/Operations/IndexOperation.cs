using System;
using System.Threading.Tasks;
using Cogworks.AzureSearch.Exceptions.IndexExceptions;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Operations
{
    public class IndexOperation<TAzureModel> : IIndexOperation<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
        private readonly AzureIndexDefinition<TAzureModel> _azureIndexDefinition;
        private readonly IIndexOperationWrapper _indexOperationWrapper;

        public IndexOperation(
            AzureIndexDefinition<TAzureModel> azureIndexDefinition,
            IIndexOperationWrapper indexOperationWrapper)
        {
            _azureIndexDefinition = azureIndexDefinition;
            _indexOperationWrapper = indexOperationWrapper;
        }

        public async Task<bool> IndexExistsAsync()
        {
            try
            {
                return await _indexOperationWrapper.ExistsAsync(_azureIndexDefinition.IndexName);
            }
            catch (Exception exception)
            {
                throw new IndexExistsException(exception.Message, exception.InnerException);
            }
        }

        public async Task IndexDeleteAsync()
        {
            try
            {
                await _indexOperationWrapper.DeleteAsync(_azureIndexDefinition.IndexName);
            }
            catch (Exception exception)
            {
                throw new IndexDeleteException(exception.Message, exception.InnerException);
            }
        }

        public async Task IndexCreateOrUpdateAsync()
        {
            try
            {

                _ = _azureIndexDefinition.CustomIndexDefinition != null
                    ? await _indexOperationWrapper.CreateOrUpdateAsync<TAzureModel>(_azureIndexDefinition.CustomIndexDefinition, true)
                    : await _indexOperationWrapper.CreateOrUpdateAsync<TAzureModel>(_azureIndexDefinition.IndexName);

            }
            catch (Exception exception)
            {
                throw new IndexCreateOrUpdateException(exception.Message, exception.InnerException);

            }
        }

        public async Task IndexClearAsync()
        {
            try
            {
                if (await IndexExistsAsync())
                {
                    await IndexDeleteAsync();
                }

                await IndexCreateOrUpdateAsync();

            }
            catch (Exception exception)
            {
                throw new IndexClearException(exception.Message, exception.InnerException);
            }
        }
    }
}