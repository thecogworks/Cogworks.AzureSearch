using System;
using System.Threading.Tasks;
using Cogworks.AzureCognitiveSearch.Exceptions;
using Cogworks.AzureCognitiveSearch.Exceptions.IndexExceptions;
using Cogworks.AzureCognitiveSearch.Interfaces.Operations;
using Cogworks.AzureCognitiveSearch.Interfaces.Wrappers;
using Cogworks.AzureCognitiveSearch.Models;
using Cogworks.AzureCognitiveSearch.Models.Dtos;

namespace Cogworks.AzureCognitiveSearch.Services
{
    public class AzureIndexOperationService<TAzureModel> : IAzureIndexOperation<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
        private readonly AzureIndexDefinition<TAzureModel> _azureIndexDefinition;
        private readonly IIndexOperationWrapper _indexOperationWrapper;

        public AzureIndexOperationService(
            AzureIndexDefinition<TAzureModel> azureIndexDefinition,
            IIndexOperationWrapper indexOperationWrapper)
        {
            _azureIndexDefinition = azureIndexDefinition;
            _indexOperationWrapper = indexOperationWrapper;
        }

        public async Task<bool> IndexExistsAsync()
            => await _indexOperationWrapper.ExistsAsync(_azureIndexDefinition.IndexName);

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