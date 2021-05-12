using System.Threading.Tasks;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Interfaces.Searches
{
    public interface IAzureSearch<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        SearchResult<TAzureModel> Search(string keyword, AzureSearchParameters azureSearchParameters);

        Task<SearchResult<TAzureModel>> SearchAsync(string keyword, AzureSearchParameters azureSearchParameters);
    }
}