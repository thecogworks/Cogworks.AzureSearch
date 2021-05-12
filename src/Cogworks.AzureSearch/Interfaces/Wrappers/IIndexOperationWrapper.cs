using System.Threading.Tasks;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Interfaces.Wrappers
{
    public interface IIndexOperationWrapper
    {
        Task<bool> ExistsAsync(string indexName);

        Task DeleteAsync(string indexName);

        Task<SearchIndex> CreateOrUpdateAsync<TAzureModel>(string indexName) where TAzureModel : class, IModel, new();

        Task<SearchIndex> CreateOrUpdateAsync<TAzureModel>(SearchIndex customIndexDefinition, bool overrideFields) where TAzureModel : class, IModel, new();
    }
}