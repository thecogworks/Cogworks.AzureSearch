using System.Threading.Tasks;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureCognitiveSearch.Models;

namespace Cogworks.AzureCognitiveSearch.Interfaces.Wrappers
{
    public interface IIndexOperationWrapper
    {
        Task<bool> ExistsAsync(string indexName);

        Task DeleteAsync(string indexName);

        Task<SearchIndex> CreateOrUpdateAsync<TAzureModel>(string indexName) where TAzureModel : class, IAzureModel, new();

        Task<SearchIndex> CreateOrUpdateAsync<TAzureModel>(SearchIndex customIndexDefinition, bool overrideFields) where TAzureModel : class, IAzureModel, new();
    }
}