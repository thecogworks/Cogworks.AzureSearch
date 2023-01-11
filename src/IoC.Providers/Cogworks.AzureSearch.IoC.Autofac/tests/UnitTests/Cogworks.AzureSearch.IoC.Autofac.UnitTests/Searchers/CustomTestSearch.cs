﻿using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.IoC.Autofac.UnitTests.Models;
using Cogworks.AzureSearch.Searchers;

namespace Cogworks.AzureSearch.IoC.Autofac.UnitTests.Searchers
{
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

            //  _ = base.Search("test", new SearchParameters());
        }
    }
}