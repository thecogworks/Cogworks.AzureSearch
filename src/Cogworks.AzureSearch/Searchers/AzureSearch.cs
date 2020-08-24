using Cogworks.AzureSearch.Interfaces;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Searchers
{
    public class AzureSearch<TAzureModel> : IAzureSearch<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
        private readonly IAzureDocumentSearch<TAzureModel> _azureSearchRepository;

        public AzureSearch(IAzureDocumentSearch<TAzureModel> azureSearchRepository)
            => _azureSearchRepository = azureSearchRepository;

        public SearchResult<TAzureModel> Search(string keyword, AzureSearchParameters azureSearchParameters)
            => _azureSearchRepository.Search(keyword, azureSearchParameters);

        public async Task<SearchResult<TAzureModel>> SearchAsync(string keyword, AzureSearchParameters azureSearchParameters)
            => await _azureSearchRepository.SearchAsync(keyword, azureSearchParameters);
    }
}