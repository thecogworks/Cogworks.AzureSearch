using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.MicrosoftIoc.UnitTests.Models;

namespace Cogworks.AzureSearch.MicrosoftIoc.UnitTests.Searchers
{
    public interface ICustomTestSearch
    {
        void SomeCustomSearchExample();
    }

    public class CustomTestSearch : AzureSearch.Searchers.AzureSearch<FirstTestDocumentModel>, ICustomTestSearch
    {
        public CustomTestSearch(IAzureDocumentSearch<FirstTestDocumentModel> azureSearchRepository) : base(azureSearchRepository)
        {
        }

        public void SomeCustomSearchExample()
        {
            // Start of custom filters/logic
            // ...
            // End of custom filters

            //  _ = base.Search("test", new AzureSearchParameters());
        }
    }
}