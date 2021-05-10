using System.Threading.Tasks;
using Cogworks.AzureCognitiveSearch.Models;
using Cogworks.AzureCognitiveSearch.Models.Dtos;

namespace Cogworks.AzureCognitiveSearch.Interfaces.Searches
{
    public interface IAzureSearch<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        SearchResult<TAzureModel> Search(string keyword, AzureSearchParameters azureSearchParameters);

        Task<SearchResult<TAzureModel>> SearchAsync(string keyword, AzureSearchParameters azureSearchParameters);
    }
}