using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.UnitTests.Models;
using Cogworks.AzureSearch.Wrappers;
using Microsoft.Rest.Azure;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Threading.Tasks;
using Xunit;

namespace Cogworks.AzureSearch.UnitTests.Repositories
{
    public class RepositoryIndexOperationTests
    {
        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoNSubstituteCustomization());

        private readonly IAzureIndexOperation<TestDocumentModel> _azureIndexOperation;

        private readonly IIndexOperationWrapper _indexOperationWrapper;
        private readonly IDocumentOperationWrapper<TestDocumentModel> _documentOperationWrapper;

        public RepositoryIndexOperationTests()
        {
            var testDocumentModelDefinition = _fixture.Create<AzureIndexDefinition<TestDocumentModel>>();

            _indexOperationWrapper = Substitute.For<IIndexOperationWrapper>();
            _documentOperationWrapper = Substitute.For<IDocumentOperationWrapper<TestDocumentModel>>();

            _azureIndexOperation = new AzureSearchRepository<TestDocumentModel>(
                testDocumentModelDefinition,
                _indexOperationWrapper,
                _documentOperationWrapper);
        }

        #region Exists Tests

        [Fact]
        public async Task Should_ReturnTrue_When_IndexExists()
        {
            // Arrange
            _indexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(true);

            // Act
            var result = await _azureIndexOperation.IndexExistsAsync();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task Should_ReturnFalse_When_IndexNotExists()
        {
            // Arrange
            _indexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(false);

            // Act
            var result = await _azureIndexOperation.IndexExistsAsync();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task Should_ThrowException_When_IssuesWithConnection()
        {
            // Arrange
            _indexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Throws(_ => new CloudException("some internal exception"));

            // Assert
            _ = await Assert.ThrowsAsync<CloudException>(async () => await _azureIndexOperation.IndexExistsAsync());
        }

        [Fact]
        public async Task Should_Not_ThrowException_When_NoIssueWithConnection()
        {
            // Arrange
            _indexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(true);

            // Act
            var indexNotExistsResult = await Record.ExceptionAsync(Should_ReturnFalse_When_IndexNotExists);
            var indexExistsResult = await Record.ExceptionAsync(Should_ReturnTrue_When_IndexExists);

            // Assert
            Assert.Null(indexNotExistsResult);
            Assert.Null(indexExistsResult);
        }

        #endregion Exists Tests
    }
}