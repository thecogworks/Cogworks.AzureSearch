using System.Threading.Tasks;
using Cogworks.AzureSearch.Extensions;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Mappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Searchers
{
    internal class Searcher<TModel> : ISearcher<TModel>
        where TModel : class, IModel, new()
    {
        private readonly IDocumentOperationWrapper<TModel> _documentOperationWrapper;

        public Searcher(IDocumentOperationWrapper<TModel> documentOperationWrapper)
            => _documentOperationWrapper = documentOperationWrapper;

        public SearchResult<TModel> Search(string keyword, SearchParameters searchParameters)
        {
            var searchText = GetSearchText(keyword);
            var parameters = ParametersMapper.Map(searchParameters);
            var results = _documentOperationWrapper.Search($"{searchText}", parameters);

            return SearchResultMapper.Map(
                results,
                searchParameters.Skip,
                searchParameters.Take);
        }

        public async Task<SearchResult<TModel>> SearchAsync(string keyword, SearchParameters searchParameters)
        {
            var searchText = GetSearchText(keyword);
            var parameters = ParametersMapper.Map(searchParameters);
            var results = await _documentOperationWrapper.SearchAsync(searchText, parameters);

            return SearchResultMapper.Map(
                results,
                searchParameters.Skip,
                searchParameters.Take);
        }

        private static string GetSearchText(string keyword)
            => keyword.EscapeHyphen().HasValue()
                ? keyword.EscapeHyphen()
                : "*";
    }
}