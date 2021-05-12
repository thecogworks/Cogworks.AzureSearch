using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Searchers
{
    public abstract class BaseDomainSearch<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
        protected IAzureSearch<TAzureModel> Searcher { get; }

        protected BaseDomainSearch(IAzureSearch<TAzureModel> search)
            => Searcher = search;

        public virtual SearchResult<TAzureModel> Search(string keyword, AzureSearchParameters azureSearchParameters)
            => Searcher.Search(keyword, azureSearchParameters);

        public async Task<SearchResult<TAzureModel>> SearchAsync(string keyword, AzureSearchParameters azureSearchParameters)
            => await Searcher.SearchAsync(keyword, azureSearchParameters);
    }
}
