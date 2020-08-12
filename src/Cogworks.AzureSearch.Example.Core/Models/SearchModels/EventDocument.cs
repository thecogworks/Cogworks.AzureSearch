using Microsoft.Azure.Search;

namespace Cogworks.AzureSearch.Example.Core.Models.SearchModels
{
    public class EventDocument : SearchModel
    {
        [IsRetrievable(true)]
        [IsFilterable]
        [IsSearchable]
        public string Name { get; set; }
    }
}