using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Interfaces.Searches
{
    public interface IAzureSearch<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        SearchResult<TAzureModel> Search(string keyword, AzureSearchParameters azureSearchParameters);

        Task<SearchResult<TAzureModel>> SearchAsync(string keyword, AzureSearchParameters azureSearchParameters);
    }
}