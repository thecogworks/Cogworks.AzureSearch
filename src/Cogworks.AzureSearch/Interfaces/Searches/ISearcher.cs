using System.Threading.Tasks;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Interfaces.Searches
{
    public interface ISearcher<TModel>
        where TModel : class, IModel, new()
    {
        SearchResult<TModel> Search(string keyword, SearchParameters searchParameters);

        Task<SearchResult<TModel>> SearchAsync(string keyword, SearchParameters searchParameters);
    }
}