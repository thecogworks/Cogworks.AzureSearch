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
        protected readonly AzureIndexDefinition<TestDocumentModel> TestDocumentModelDefinition;
        protected readonly IAzureSearch<TestDocumentModel> Search;

        protected readonly IDocumentOperation<TestDocumentModel> DocumentOperationService;
        protected readonly IIndexOperation<TestDocumentModel> IndexOperationService;

        protected TestBase()
        {
            TestDocumentModelDefinition = Fixture.Create<AzureIndexDefinition<TestDocumentModel>>();

            IndexOperationWrapper = Substitute.For<IIndexOperationWrapper>();
            DocumentOperationWrapper = Substitute.For<IDocumentOperationWrapper<TestDocumentModel>>();

            Search = Substitute.For<IAzureSearch<TestDocumentModel>>();

            DocumentOperationService = new DocumentOperation<TestDocumentModel>(
                DocumentOperationWrapper);

            IndexOperationService = new IndexOperation<TestDocumentModel>(
                TestDocumentModelDefinition,
                IndexOperationWrapper);
        }
    }
}