using System.Threading.Tasks;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Interfaces.Searches
{
    public interface IAzureSearch<TAzureModel> where TAzureModel : class, IModel, new()
    {
        SearchResult<TAzureModel> Search(string keyword, SearchParameters searchParameters);

        Task<SearchResult<TAzureModel>> SearchAsync(string keyword, SearchParameters searchParameters);
    }
}