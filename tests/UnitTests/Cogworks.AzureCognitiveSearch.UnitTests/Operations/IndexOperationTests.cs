using System;
using System.Threading.Tasks;
using AutoFixture;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureCognitiveSearch.Exceptions.IndexExceptions;
using Cogworks.AzureCognitiveSearch.Interfaces.Operations;
using Cogworks.AzureCognitiveSearch.Models;
using Cogworks.AzureCognitiveSearch.Repositories;
using Cogworks.AzureCognitiveSearch.Services;
using Cogworks.AzureCognitiveSearch.UnitTests.Models;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Cogworks.AzureCognitiveSearch.UnitTests.Operations
{
    public class IndexOperationTests : TestBase
    {
        private readonly IAzureIndexOperation<TestDocumentModel> _azureIndexOperation;

        public IndexOperationTests()
            => _azureIndexOperation = new AzureSearchRepository<TestDocumentModel>(
                AzureIndexOperationService,
                AzureDocumentOperationService,
                Search);

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
                .Throws(_ => new IndexExistsException(
                    "Test Error",
                    Fixture.Create<Exception>()));

            // Assert
            var domainException = await Assert.ThrowsAsync<IndexExistsException>(async () => await _azureIndexOperation.IndexExistsAsync());

            Assert.NotNull(domainException);
            Assert.Equal("Test Error", domainException.Message);
            Assert.NotNull(domainException.InnerException);
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

            var deleteResult = await Record.ExceptionAsync(async () => await _azureIndexOperation.IndexDeleteAsync());

            // Assert
            Assert.Null(deleteResult);
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
        public async Task Should_ThrowException_When_IssuesWithConnection_On_DeletingIndex()
        {
            // Arrange
            _ = IndexOperationWrapper.DeleteAsync(Arg.Any<string>())
                .Throws(_ => new IndexDeleteException(
                    "Test Error",
                    Fixture.Create<Exception>()));

            // Act
            var domainException = await Assert.ThrowsAsync<IndexDeleteException>(async () => await _azureIndexOperation.IndexDeleteAsync());

            // Assert
            Assert.NotNull(domainException);
            Assert.Equal("Test Error", domainException.Message);
            Assert.NotNull(domainException.InnerException);
        }

        #endregion Delete Tests

        #region Index Create or Update Tests

        [Fact]
        public async Task Should_CreateOrUpdateIndex()
        {
            // Arrange
            var createdOrUpdatedIndex = new SearchIndex(TestDocumentModelDefinition.IndexName);

            _ = IndexOperationWrapper.CreateOrUpdateAsync<TestDocumentModel>(Arg.Any<string>())
                .Returns(createdOrUpdatedIndex);

            // Act
            var indexResult = await Record.ExceptionAsync(async () => await _azureIndexOperation.IndexCreateOrUpdateAsync());

            // Assert
            Assert.Null(indexResult);

        }

        [Fact]
        public async Task Should_CreateOrUpdateCustomIndex()
        {
            // Arrange
            var index = new SearchIndex(Fixture.Create<string>());

            var customModelDefinition = new AzureIndexDefinition<TestDocumentModel>(index);

            var customIndexOperationService = new AzureIndexOperationService<TestDocumentModel>(
                customModelDefinition,
                IndexOperationWrapper);

            var azureIndexOperation = new AzureSearchRepository<TestDocumentModel>(
                customIndexOperationService,
                AzureDocumentOperationService,
                Search);

            // Act
            var indexResult = await Record.ExceptionAsync(async () => await azureIndexOperation.IndexCreateOrUpdateAsync());

            // Assert
            Assert.Null(indexResult);
        }

        [Fact]
        public async Task Should_ThrowException_When_IssueOnCreatingOrUpdatingIndex()
        {
            // Arrange
            _ = IndexOperationWrapper.CreateOrUpdateAsync<TestDocumentModel>(Arg.Any<string>())
                .Throws(_ => new IndexCreateOrUpdateException(
                    "Test Error",
                    Fixture.Create<Exception>()));

            // Act
            var domainException = await Assert.ThrowsAsync<IndexCreateOrUpdateException>(async () => await _azureIndexOperation.IndexCreateOrUpdateAsync());

            // Assert
            Assert.NotNull(domainException);
            Assert.Equal("Test Error", domainException.Message);
            Assert.NotNull(domainException.InnerException);
        }

        [Fact]
        public async Task Should_ThrowException_When_IssueOnCreatingOrUpdatingCustomIndex()
        {
            // Arrange
            var index = new SearchIndex(Fixture.Create<string>());

            var customModelDefinition = new AzureIndexDefinition<TestDocumentModel>(index);

            var customIndexOperationService = new AzureIndexOperationService<TestDocumentModel>(
                customModelDefinition,
                IndexOperationWrapper);

            var azureIndexOperation = new AzureSearchRepository<TestDocumentModel>(
                customIndexOperationService,
                AzureDocumentOperationService,
                Search);

            _ = IndexOperationWrapper.CreateOrUpdateAsync<TestDocumentModel>(Arg.Any<SearchIndex>(), Arg.Any<bool>())
                .Throws(_ => new IndexCreateOrUpdateException(
                    "Test Error",
                    Fixture.Create<Exception>()));

            // Act
            var domainException = await Assert.ThrowsAsync<IndexCreateOrUpdateException>(async () => await azureIndexOperation.IndexCreateOrUpdateAsync());

            // Assert
            Assert.NotNull(domainException);
            Assert.Equal("Test Error", domainException.Message);
            Assert.NotNull(domainException.InnerException);
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
            var indexResult = await Record.ExceptionAsync(async () => await _azureIndexOperation.IndexClearAsync());

            // Assert
            Assert.Null(indexResult);
        }

        [Fact]
        public async Task Should_ClearIndex_When_IndexNotExists()
        {
            // Arrange
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(false);

            // Act
            var indexResult = await Record.ExceptionAsync(async () => await _azureIndexOperation.IndexClearAsync());

            // Assert
            Assert.Null(indexResult);
        }

        [Fact]
        public async Task Should_ThrowException_When_IssueWithConnectionOnClearingIndex()
        {
            // Arrange
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(true);

            _ = IndexOperationWrapper.DeleteAsync(Arg.Any<string>())
                .Throws(_ => new IndexClearException(
                    "Test Error",
                    Fixture.Create<Exception>()));

            // Act
            var domainException = await Record.ExceptionAsync(async () => await _azureIndexOperation.IndexClearAsync());

            // Assert
            Assert.NotNull(domainException);
            Assert.Equal("Test Error", domainException.Message);
            Assert.NotNull(domainException.InnerException);


            // Arrange
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Throws(_ => new IndexExistsException(
                    "Test Error",
                    Fixture.Create<Exception>()));

            _ = IndexOperationWrapper.CreateOrUpdateAsync<TestDocumentModel>(Arg.Any<string>())
                .Throws(_ => new IndexCreateOrUpdateException(
                    "Test Error",
                    Fixture.Create<Exception>()));

            // Act
            domainException = await Record.ExceptionAsync(async () => await _azureIndexOperation.IndexClearAsync());

            // Assert
            Assert.NotNull(domainException);
            Assert.Equal("Test Error", domainException.Message);
            Assert.NotNull(domainException.InnerException);
        }

        #endregion Index Clear Tests
    }
}