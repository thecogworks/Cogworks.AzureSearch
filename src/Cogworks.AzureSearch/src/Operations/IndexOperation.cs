using System;
using System.Threading.Tasks;
using Cogworks.AzureSearch.Exceptions.IndexExceptions;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Operations
{
    public class IndexOperation<TModel> : IIndexOperation<TModel>
        where TModel : class, IModel, new()
    {
        private readonly IndexDefinition<TModel> _indexDefinition;
        private readonly IIndexOperationWrapper _indexOperationWrapper;

        public IndexOperation(
            IndexDefinition<TModel> indexDefinition,
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
                var domainException = new IndexExistsException(exception.Message, exception.InnerException);
                throw domainException;
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
                var domainException = new IndexDeleteException(exception.Message, exception.InnerException);
                throw domainException;
            }
        }

        public async Task IndexCreateOrUpdateAsync()
        {
            try
            {

                _ = _indexDefinition.CustomIndexDefinition != null
                    ? await _indexOperationWrapper.CreateOrUpdateAsync<TModel>(_indexDefinition.CustomIndexDefinition, true)
                    : await _indexOperationWrapper.CreateOrUpdateAsync<TModel>(_indexDefinition.IndexName);

            }
            catch (Exception exception)
            {
                var domainException = new IndexCreateOrUpdateException(exception.Message, exception.InnerException);
                throw domainException;

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
                var domainException = new IndexClearException(exception.Message, exception.InnerException);
                throw domainException;
            }
        }
    }
}