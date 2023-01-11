using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.IoC.Umbraco.UnitTests.Models;
using Cogworks.AzureSearch.Searchers;

namespace Cogworks.AzureSearch.IoC.Umbraco.UnitTests.Searchers
{
#pragma warning disable SA1649 // SA1649FileNameMustMatchTypeName
    public interface ICustomTestSearch
    {
        void SomeCustomSearchExample();
    }

    public class CustomTestSearch : BaseDomainSearch<FirstTestDocumentModel>, ICustomTestSearch
    {
        public CustomTestSearch(ISearcher<FirstTestDocumentModel> search) : base(search)
        {
        }

        public void SomeCustomSearchExample()
        {
            // Start of custom filters/logic
            // ...
            // End of custom filters

            // _ = base.Search("test", new SearchParameters());
        }
    }
#pragma warning restore SA1649 // SA1649FileNameMustMatchTypeName
}