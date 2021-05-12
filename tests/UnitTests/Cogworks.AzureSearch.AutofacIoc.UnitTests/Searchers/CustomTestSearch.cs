using Cogworks.AzureSearch.AutofacIoc.UnitTests.Models;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Searchers;

namespace Cogworks.AzureSearch.AutofacIoc.UnitTests.Searchers
{
    public interface ICustomTestSearch
    {
        void SomeCustomSearchExample();
    }

    public class CustomTestSearch : BaseDomainSearch<FirstTestDocumentModel>, ICustomTestSearch
    {
        public CustomTestSearch(IAzureSearch<FirstTestDocumentModel> search) : base(search)
        {
        }

        public void SomeCustomSearchExample()
        {
            // Start of custom filters/logic
            // ...
            // End of custom filters

            //  _ = base.Search("test", new SearchParameters());
        }
    }
}