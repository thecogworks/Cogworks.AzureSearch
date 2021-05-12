using System;
using System.Threading.Tasks;
using Cogworks.AzureSearch.Exceptions.IndexExceptions;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Operations
{
    public class IndexOperation<TAzureModel> : IIndexOperation<TAzureModel>
        where TAzureModel : class, IModel, new()
    {
        private readonly IndexDefinition<TAzureModel> _indexDefinition;
        private readonly IIndexOperationWrapper _indexOperationWrapper;

        public IndexOperation(
            IndexDefinition<TAzureModel> indexDefinition,
            IIndexOperationWrapper indexOperationWrapper)
        {
            _indexDefinition = indexDefinition;
            _indexOperationWrapper = indexOperationWrapper;
        }

        public async Task<bool> IndexExistsAsync()
        {
            try
            {
                return await _indexOperationWrapper.ExistsAsync(_indexDefinition.IndexName);
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
                await _indexOperationWrapper.DeleteAsync(_indexDefinition.IndexName);
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

                _ = _indexDefinition.CustomIndexDefinition != null
                    ? await _indexOperationWrapper.CreateOrUpdateAsync<TAzureModel>(_indexDefinition.CustomIndexDefinition, true)
                    : await _indexOperationWrapper.CreateOrUpdateAsync<TAzureModel>(_indexDefinition.IndexName);

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