using AutoFixture;
using AutoFixture.AutoNSubstitute;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.UnitTests.Models;
using Cogworks.AzureSearch.Wrappers;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Threading.Tasks;
using Xunit;

namespace Cogworks.AzureSearch.UnitTests.Repositories
{
    public class RepositoryIndexOperationTests
    {
        private const string _azureWrapperException = "some internal exception";

        private readonly IFixture _fixture = new Fixture()
            .Customize(new AutoNSubstituteCustomization());

        private readonly IAzureIndexOperation<TestDocumentModel> _azureIndexOperation;

        private readonly IIndexOperationWrapper _indexOperationWrapper;
        private readonly IDocumentOperationWrapper<TestDocumentModel> _documentOperationWrapper;
        private readonly AzureIndexDefinition<TestDocumentModel> _testDocumentModelDefinition;

        public RepositoryIndexOperationTests()
        {
            _testDocumentModelDefinition = _fixture.Create<AzureIndexDefinition<TestDocumentModel>>();

            _indexOperationWrapper = Substitute.For<IIndexOperationWrapper>();
            _documentOperationWrapper = Substitute.For<IDocumentOperationWrapper<TestDocumentModel>>();

            _azureIndexOperation = new AzureSearchRepository<TestDocumentModel>(
                _testDocumentModelDefinition,
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
                .Throws(_ => new CloudException(_azureWrapperException));

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

        #region Delete Tests

        [Fact]
        public async Task Should_DeleteIndex_When_IndexExists()
        {
            // Act
            var deleteResult = await _azureIndexOperation.IndexDeleteAsync();

            // Assert
            Assert.NotNull(deleteResult);
            Assert.True(deleteResult.Succeeded);
            Assert.Equal($"Index {_testDocumentModelDefinition.IndexName} successfully deleted.", deleteResult.Message);
        }

        [Fact]
        public async Task Should_Not_DeleteIndex_When_IndexNotExists()
        {
            // Arrange
            _indexOperationWrapper.DeleteAsync(Arg.Any<string>())
                .Throws(_ => new CloudException(_azureWrapperException));

            // Act
            var deleteResult = await _azureIndexOperation.IndexDeleteAsync();

            // Assert
            Assert.NotNull(deleteResult);
            Assert.False(deleteResult.Succeeded);
            Assert.Equal($"An issue occured on deleting index: {_testDocumentModelDefinition.IndexName}. More information: {_azureWrapperException}", deleteResult.Message);
        }

        [Fact]
        public async Task Should_Not_ThrowException_When_DeletingExistingIndex()
        {
            // Act
            var indexDeletingResult = await Record.ExceptionAsync(Should_DeleteIndex_When_IndexExists);

            // Assert
            Assert.Null(indexDeletingResult);
        }

        [Fact]
        public async Task Should_Not_ThrowException_When_DeletingNotExistingIndex()
        {
            // Act
            var indexDeletingResult = await Record.ExceptionAsync(Should_Not_DeleteIndex_When_IndexNotExists);

            // Assert
            Assert.Null(indexDeletingResult);
        }

        #endregion Delete Tests

        #region Index Create or Update Tests

        [Fact]
        public async Task Should_CreateOrUpdateIndex()
        {
            // Arrange
            var createdOrUpdatedIndex = new Index
            {
                Name = _testDocumentModelDefinition.IndexName
            };

            _indexOperationWrapper.CreateOrUpdateAsync<TestDocumentModel>(Arg.Any<string>())
                .Returns(createdOrUpdatedIndex);

            // Act
            var operationResult = await _azureIndexOperation.IndexCreateOrUpdateAsync();

            // Assert
            Assert.NotNull(operationResult);
            Assert.True(operationResult.Succeeded);
            Assert.Equal($"Index {_testDocumentModelDefinition.IndexName} successfully created or updated.", operationResult.Message);
        }

        [Fact]
        public async Task Should_Not_ThrowException_When_IssueOnCreatingOrUpdatingIndex()
        {
            // Arrange
            _indexOperationWrapper.CreateOrUpdateAsync<TestDocumentModel>(Arg.Any<string>())
                .Throws(_ => new CloudException(_azureWrapperException));

            // Act
            var operationResult = await _azureIndexOperation.IndexCreateOrUpdateAsync();

            // Assert
            Assert.NotNull(operationResult);
            Assert.False(operationResult.Succeeded);
            Assert.Equal($"An issue occured on creating or updating index: {_testDocumentModelDefinition.IndexName}. More information: {_azureWrapperException}", operationResult.Message);
        }

        #endregion Index Create or Update Tests

        #region Index Clear Tests

        [Fact]
        public async Task Should_ClearIndex_When_IndexExists()
        {
            // Arrange
            _indexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(true);

            // Act
            var indexOperationResult = await _azureIndexOperation.IndexClearAsync();

            // Assert
            Assert.NotNull(indexOperationResult);
            Assert.True(indexOperationResult.Succeeded);
            Assert.Equal($"Index {_testDocumentModelDefinition.IndexName} successfully cleared.", indexOperationResult.Message);
        }

        [Fact]
        public async Task Should_ClearIndex_When_IndexNotExists()
        {
            // Arrange
            _indexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(false);

            // Act
            var indexOperationResult = await _azureIndexOperation.IndexClearAsync();

            // Assert
            Assert.NotNull(indexOperationResult);
            Assert.True(indexOperationResult.Succeeded);
            Assert.Equal($"Index {_testDocumentModelDefinition.IndexName} successfully cleared.", indexOperationResult.Message);
        }

        [Fact]
        public async Task Should_Not_ThrowException_When_IssueWithConnectionOnClearingIndex()
        {
            // Arrange
            _indexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(true);

            _indexOperationWrapper.DeleteAsync(Arg.Any<string>())
                .Throws(_ => new CloudException(_azureWrapperException));

            // Act
            var indexOperationResult = await _azureIndexOperation.IndexClearAsync();

            // Assert
            Assert.NotNull(indexOperationResult);
            Assert.False(indexOperationResult.Succeeded);
            Assert.Equal($"An issue occured on clearing index: {_testDocumentModelDefinition.IndexName}. Could not delete existing index.", indexOperationResult.Message);

            // Arrange
            _indexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Throws(_ => new CloudException(_azureWrapperException));

            _indexOperationWrapper.CreateOrUpdateAsync<TestDocumentModel>(Arg.Any<string>())
                .Throws(_ => new CloudException(_azureWrapperException));

            // Act
            indexOperationResult = await _azureIndexOperation.IndexClearAsync();

            // Assert
            Assert.NotNull(indexOperationResult);
            Assert.False(indexOperationResult.Succeeded);
            Assert.Equal($"An issue occured on clearing index: {_testDocumentModelDefinition.IndexName}. Could not create index.", indexOperationResult.Message);
        }

        #endregion Index Clear Tests
    }
}