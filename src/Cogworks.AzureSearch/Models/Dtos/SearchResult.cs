using System.Collections.Generic;

namespace Cogworks.AzureSearch.Models.Dtos
{
    public class SearchResult<TModel> where TModel : class, IAzureModel, new()
    {
        public long TotalCount { get; set; }

        public IEnumerable<SearchResultItem<TModel>> Results { get; set; }

        public bool HasMoreItems { get; set; }
    }
}