using System.Linq;
using System.Threading.Tasks;
using Cogworks.AzureCognitiveSearch.Extensions;
using Cogworks.AzureCognitiveSearch.Interfaces.Searches;
using Cogworks.AzureCognitiveSearch.Interfaces.Wrappers;
using Cogworks.AzureCognitiveSearch.Mappers;
using Cogworks.AzureCognitiveSearch.Models;
using Cogworks.AzureCognitiveSearch.Models.Dtos;

namespace Cogworks.AzureCognitiveSearch.Searchers
{
    public class AzureSearch<TAzureModel> : IAzureSearch<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
        private readonly IDocumentOperationWrapper<TAzureModel> _documentOperationWrapper;

        public AzureSearch(IDocumentOperationWrapper<TAzureModel> documentOperationWrapper)
            => _documentOperationWrapper = documentOperationWrapper;

        public SearchResult<TAzureModel> Search(string keyword, AzureSearchParameters azureSearchParameters)
        {
            var searchText = GetSearchText(keyword);
            var parameters = AzureSearchParametersMapper.Map(azureSearchParameters);
            var results = _documentOperationWrapper.Search($"{searchText}", parameters);

            return SearchResultMapper.Map(
                results,
                azureSearchParameters.Skip,
                azureSearchParameters.Take);
        }

        public async Task<SearchResult<TAzureModel>> SearchAsync(string keyword, AzureSearchParameters azureSearchParameters)
        {
            var searchText = GetSearchText(keyword);
            var parameters = AzureSearchParametersMapper.Map(azureSearchParameters);
            var results = await _documentOperationWrapper.SearchAsync(searchText, parameters);

            return SearchResultMapper.Map(
                results,
                azureSearchParameters.Skip,
                azureSearchParameters.Take);
        }

        private static string GetSearchText(string keyword)
            => keyword.EscapeHyphen().HasValue()
                ? keyword.EscapeHyphen()
                : "*";
    }
}