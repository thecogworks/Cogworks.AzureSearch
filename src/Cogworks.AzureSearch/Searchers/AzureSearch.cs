using System.Threading.Tasks;
using Cogworks.AzureSearch.Extensions;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Mappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Searchers
{
    internal class AzureSearch<TAzureModel> : IAzureSearch<TAzureModel>
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