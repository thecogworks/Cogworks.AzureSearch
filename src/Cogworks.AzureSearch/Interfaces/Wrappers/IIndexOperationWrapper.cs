using System.Threading.Tasks;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Interfaces.Wrappers
{
    public interface IIndexOperationWrapper
    {
        Task<bool> ExistsAsync(string indexName);

        Task DeleteAsync(string indexName);

        Task<SearchIndex> CreateOrUpdateAsync<TModel>(string indexName) where TModel : class, IModel, new();

        Task<SearchIndex> CreateOrUpdateAsync<TModel>(SearchIndex customIndexDefinition, bool overrideFields) where TModel : class, IModel, new();
    }
}