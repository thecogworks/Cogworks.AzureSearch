using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Searchers
{
    public abstract class BaseDomainSearch<TModel>
        where TModel : class, IModel, new()
    {
        protected ISearcher<TModel> Searcher { get; }

        protected BaseDomainSearch(ISearcher<TModel> search)
            => Searcher = search;

        public virtual SearchResult<TModel> Search(string keyword, SearchParameters searchParameters)
            => Searcher.Search(keyword, searchParameters);

        public async Task<SearchResult<TModel>> SearchAsync(string keyword, SearchParameters searchParameters)
            => await Searcher.SearchAsync(keyword, searchParameters);
    }
}
