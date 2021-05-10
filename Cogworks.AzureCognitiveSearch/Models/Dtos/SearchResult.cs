using System.Collections.Generic;

namespace Cogworks.AzureCognitiveSearch.Models.Dtos
{
    public class SearchResult<TModel> where TModel : class, IAzureModel, new()
    {
        public long TotalCount { get; set; }

        public IEnumerable<SearchResultItem<TModel>> Results { get; set; }

        public bool HasMoreItems { get; set; }
    }
}