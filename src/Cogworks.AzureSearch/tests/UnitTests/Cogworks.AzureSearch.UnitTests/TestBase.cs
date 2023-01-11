using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Operations;
using Cogworks.AzureSearch.UnitTests.Models;
using NSubstitute;

namespace Cogworks.AzureSearch.UnitTests
{
    public abstract class TestBase
    {
        protected const string AzureWrapperException = "some internal exception";

        protected readonly IFixture Fixture = new Fixture()
            .Customize(new AutoNSubstituteCustomization());

        protected readonly IIndexOperationWrapper IndexOperationWrapper;
        protected readonly IDocumentOperationWrapper<TestDocumentModel> DocumentOperationWrapper;
        protected readonly IndexDefinition<TestDocumentModel> TestDocumentModelDefinition;
        protected readonly ISearcher<TestDocumentModel> Search;

        protected readonly IDocumentOperation<TestDocumentModel> DocumentOperation;
        protected readonly IIndexOperation<TestDocumentModel> IndexOperation;

        protected TestBase()
        {
            TestDocumentModelDefinition = Fixture.Create<IndexDefinition<TestDocumentModel>>();

            IndexOperationWrapper = Substitute.For<IIndexOperationWrapper>();
            DocumentOperationWrapper = Substitute.For<IDocumentOperationWrapper<TestDocumentModel>>();

            Search = Substitute.For<ISearcher<TestDocumentModel>>();

            DocumentOperation = new DocumentOperation<TestDocumentModel>(
                DocumentOperationWrapper);

            IndexOperation = new IndexOperation<TestDocumentModel>(
                TestDocumentModelDefinition,
                IndexOperationWrapper);
        }
    }
}