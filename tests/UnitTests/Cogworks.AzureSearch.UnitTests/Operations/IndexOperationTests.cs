using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.UnitTests.Models;
using Microsoft.Azure.Search.Models;
using Microsoft.Rest.Azure;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Threading.Tasks;
using Xunit;

namespace Cogworks.AzureSearch.UnitTests.Operations
{
    public class IndexOperationTests : TestBase
    {
        private readonly IAzureIndexOperation<TestDocumentModel> _azureIndexOperation;

        public IndexOperationTests()
            => _azureIndexOperation = new AzureSearchRepository<TestDocumentModel>(
                TestDocumentModelDefinition,
                IndexOperationWrapper,
                DocumentOperationWrapper);

        #region Exists Tests

        [Fact]
        public async Task Should_ReturnTrue_When_IndexExists()
        {
            // Arrange
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
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
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
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
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Throws(_ => new CloudException(AzureWrapperException));

            // Assert
            _ = await Assert.ThrowsAsync<CloudException>(async () => await _azureIndexOperation.IndexExistsAsync());
        }

        [Fact]
        public async Task Should_Not_ThrowException_When_NoIssueWithConnection()
        {
            // Arrange
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
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
            Assert.Equal($"Index {TestDocumentModelDefinition.IndexName} successfully deleted.", deleteResult.Message);
        }

        [Fact]
        public async Task Should_Not_DeleteIndex_When_IndexNotExists()
        {
            // Arrange
            _ = IndexOperationWrapper.DeleteAsync(Arg.Any<string>())
                .Throws(_ => new CloudException(AzureWrapperException));

            // Act
            var deleteResult = await _azureIndexOperation.IndexDeleteAsync();

            // Assert
            Assert.NotNull(deleteResult);
            Assert.False(deleteResult.Succeeded);
            Assert.Equal($"An issue occured on deleting index: {TestDocumentModelDefinition.IndexName}. More information: {AzureWrapperException}", deleteResult.Message);
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
                Name = TestDocumentModelDefinition.IndexName
            };

            _ = IndexOperationWrapper.CreateOrUpdateAsync<TestDocumentModel>(Arg.Any<string>())
                .Returns(createdOrUpdatedIndex);

            // Act
            var operationResult = await _azureIndexOperation.IndexCreateOrUpdateAsync();

            // Assert
            Assert.NotNull(operationResult);
            Assert.True(operationResult.Succeeded);
            Assert.Equal($"Index {TestDocumentModelDefinition.IndexName} successfully created or updated.", operationResult.Message);
        }

        [Fact]
        public async Task Should_CreateOrUpdateCustomIndex()
        {
            // Arrange
            var createdOrUpdatedIndex = new Index
            {
                Name = TestDocumentModelDefinition.IndexName
            };

            _ = IndexOperationWrapper.CreateOrUpdateAsync<TestDocumentModel>(Arg.Any<Index>(), Arg.Any<bool>())
                .Returns(createdOrUpdatedIndex);

            // Act
            var operationResult = await _azureIndexOperation.IndexCreateOrUpdateAsync();

            // Assert
            Assert.NotNull(operationResult);
            Assert.True(operationResult.Succeeded);
            Assert.Equal($"Index {TestDocumentModelDefinition.IndexName} successfully created or updated.", operationResult.Message);
        }

        [Fact]
        public async Task Should_Not_ThrowException_When_IssueOnCreatingOrUpdatingIndex()
        {
            // Arrange
            _ = IndexOperationWrapper.CreateOrUpdateAsync<TestDocumentModel>(Arg.Any<string>())
                .Throws(_ => new CloudException(AzureWrapperException));

            // Act
            var operationResult = await _azureIndexOperation.IndexCreateOrUpdateAsync();

            // Assert
            Assert.NotNull(operationResult);
            Assert.False(operationResult.Succeeded);
            Assert.Equal($"An issue occured on creating or updating index: {TestDocumentModelDefinition.IndexName}. More information: {AzureWrapperException}", operationResult.Message);
        }

        #endregion Index Create or Update Tests

        #region Index Clear Tests

        [Fact]
        public async Task Should_ClearIndex_When_IndexExists()
        {
            // Arrange
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(true);

            // Act
            var indexOperationResult = await _azureIndexOperation.IndexClearAsync();

            // Assert
            Assert.NotNull(indexOperationResult);
            Assert.True(indexOperationResult.Succeeded);
            Assert.Equal($"Index {TestDocumentModelDefinition.IndexName} successfully cleared.", indexOperationResult.Message);
        }

        [Fact]
        public async Task Should_ClearIndex_When_IndexNotExists()
        {
            // Arrange
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(false);

            // Act
            var indexOperationResult = await _azureIndexOperation.IndexClearAsync();

            // Assert
            Assert.NotNull(indexOperationResult);
            Assert.True(indexOperationResult.Succeeded);
            Assert.Equal($"Index {TestDocumentModelDefinition.IndexName} successfully cleared.", indexOperationResult.Message);
        }

        [Fact]
        public async Task Should_Not_ThrowException_When_IssueWithConnectionOnClearingIndex()
        {
            // Arrange
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(true);

            _ = IndexOperationWrapper.DeleteAsync(Arg.Any<string>())
                .Throws(_ => new CloudException(AzureWrapperException));

            // Act
            var indexOperationResult = await _azureIndexOperation.IndexClearAsync();

            // Assert
            Assert.NotNull(indexOperationResult);
            Assert.False(indexOperationResult.Succeeded);
            Assert.Equal($"An issue occured on clearing index: {TestDocumentModelDefinition.IndexName}. Could not delete existing index.", indexOperationResult.Message);

            // Arrange
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Throws(_ => new CloudException(AzureWrapperException));

            _ = IndexOperationWrapper.CreateOrUpdateAsync<TestDocumentModel>(Arg.Any<string>())
                .Throws(_ => new CloudException(AzureWrapperException));

            // Act
            indexOperationResult = await _azureIndexOperation.IndexClearAsync();

            // Assert
            Assert.NotNull(indexOperationResult);
            Assert.False(indexOperationResult.Succeeded);
            Assert.Equal($"An issue occured on clearing index: {TestDocumentModelDefinition.IndexName}. Could not create index.", indexOperationResult.Message);
        }

        #endregion Index Clear Tests
    }
}