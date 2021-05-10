using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Cogworks.AzureCognitiveSearch.Interfaces.Operations;
using Cogworks.AzureCognitiveSearch.Interfaces.Searches;
using Cogworks.AzureCognitiveSearch.Interfaces.Wrappers;
using Cogworks.AzureCognitiveSearch.Models;
using Cogworks.AzureCognitiveSearch.Services;
using Cogworks.AzureCognitiveSearch.UnitTests.Models;
using NSubstitute;

namespace Cogworks.AzureCognitiveSearch.UnitTests
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

        protected readonly IAzureDocumentOperation<TestDocumentModel> AzureDocumentOperationService;
        protected readonly IAzureIndexOperation<TestDocumentModel> AzureIndexOperationService;

        protected TestBase()
        {
            TestDocumentModelDefinition = Fixture.Create<AzureIndexDefinition<TestDocumentModel>>();

            IndexOperationWrapper = Substitute.For<IIndexOperationWrapper>();
            DocumentOperationWrapper = Substitute.For<IDocumentOperationWrapper<TestDocumentModel>>();

            Search = Substitute.For<IAzureSearch<TestDocumentModel>>();

            AzureDocumentOperationService = new AzureDocumentOperationService<TestDocumentModel>(
                DocumentOperationWrapper);

            AzureIndexOperationService = new AzureIndexOperationService<TestDocumentModel>(
                TestDocumentModelDefinition,
                IndexOperationWrapper);
        }
    }
}