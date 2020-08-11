using Cogworks.AzureSearch.Example.Core.Constants.Models;
using Cogworks.AzureSearch.Example.Core.Models.SearchModels;
using Cogworks.AzureSearch.Filters;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.Searchers;

namespace Cogworks.AzureSearch.Example.Core.Services.SearchServices
{
    public interface IEventSearch
    {
        void SomeSearch();
    }

    public class EventSearch : AzureSearch<EventDocument>, IEventSearch
    {
        public EventSearch(IAzureDocumentSearch<EventDocument> azureSearchRepository) : base(azureSearchRepository)
        {
        }

        public void SomeSearch()
        {
            var domainFilter = EventDocumentConstants.Name.EqualsValue("SomeDocumentName");
            var azureSearchParameters = new AzureSearchParameters
            {
                Filter = domainFilter
            };

            var result = Search("SomeTestKeyword", azureSearchParameters);
        }
    }
}