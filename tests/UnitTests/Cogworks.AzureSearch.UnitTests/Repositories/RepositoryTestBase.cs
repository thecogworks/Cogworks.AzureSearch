using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.UnitTests.Models;
using Cogworks.AzureSearch.Wrappers;
using NSubstitute;

namespace Cogworks.AzureSearch.UnitTests.Repositories
{
    public abstract class RepositoryTestBase
    {
        protected const string AzureWrapperException = "some internal exception";

        protected readonly IFixture Fixture = new Fixture()
            .Customize(new AutoNSubstituteCustomization());

        protected readonly IIndexOperationWrapper IndexOperationWrapper;
        protected readonly IDocumentOperationWrapper<TestDocumentModel> DocumentOperationWrapper;
        protected readonly AzureIndexDefinition<TestDocumentModel> TestDocumentModelDefinition;

        protected RepositoryTestBase()
        {
            TestDocumentModelDefinition = Fixture.Create<AzureIndexDefinition<TestDocumentModel>>();

            IndexOperationWrapper = Substitute.For<IIndexOperationWrapper>();
            DocumentOperationWrapper = Substitute.For<IDocumentOperationWrapper<TestDocumentModel>>();
        }
    }
}