using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Searchers
{
    public abstract class BaseDomainSearch<TAzureModel>
        where TAzureModel : class, IModel, new()
    {
        protected IAzureSearch<TAzureModel> Searcher { get; }

        protected BaseDomainSearch(IAzureSearch<TAzureModel> search)
            => Searcher = search;

        public virtual SearchResult<TAzureModel> Search(string keyword, SearchParameters searchParameters)
            => Searcher.Search(keyword, searchParameters);

        public async Task<SearchResult<TAzureModel>> SearchAsync(string keyword, SearchParameters searchParameters)
            => await Searcher.SearchAsync(keyword, searchParameters);
    }
}
