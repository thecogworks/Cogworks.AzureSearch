using System;
using System.Threading.Tasks;
using AutoFixture;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.Exceptions.IndexExceptions;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Operations;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.UnitTests.Models;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Cogworks.AzureSearch.UnitTests.Operations
{
    public class IndexOperationTests : TestBase
    {
        private readonly IIndexOperation<TestDocumentModel> _indexOperation;

        public IndexOperationTests()
            => _indexOperation = new SearchRepository<TestDocumentModel>(
                IndexOperationService,
                DocumentOperationService,
                Search);

        #region Exists Tests

        [Fact]
        public async Task Should_ReturnTrue_When_IndexExists()
        {
            // Arrange
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(true);

            // Act
            var result = await _indexOperation.IndexExistsAsync();

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
            var result = await _indexOperation.IndexExistsAsync();

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
            var domainException = await Assert.ThrowsAsync<IndexExistsException>(async () => await _indexOperation.IndexExistsAsync());

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

            var deleteResult = await Record.ExceptionAsync(async () => await _indexOperation.IndexDeleteAsync());

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
            var domainException = await Assert.ThrowsAsync<IndexDeleteException>(async () => await _indexOperation.IndexDeleteAsync());

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
            var indexResult = await Record.ExceptionAsync(async () => await _indexOperation.IndexCreateOrUpdateAsync());

            // Assert
            Assert.Null(indexResult);

        }

        [Fact]
        public async Task Should_CreateOrUpdateCustomIndex()
        {
            // Arrange
            var index = new SearchIndex(Fixture.Create<string>());

            var customModelDefinition = new AzureIndexDefinition<TestDocumentModel>(index);

            var customIndexOperationService = new IndexOperation<TestDocumentModel>(
                customModelDefinition,
                IndexOperationWrapper);

            var azureIndexOperation = new SearchRepository<TestDocumentModel>(
                customIndexOperationService,
                DocumentOperationService,
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
            var domainException = await Assert.ThrowsAsync<IndexCreateOrUpdateException>(async () => await _indexOperation.IndexCreateOrUpdateAsync());

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

            var customIndexOperationService = new IndexOperation<TestDocumentModel>(
                customModelDefinition,
                IndexOperationWrapper);

            var azureIndexOperation = new SearchRepository<TestDocumentModel>(
                customIndexOperationService,
                DocumentOperationService,
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
            var indexResult = await Record.ExceptionAsync(async () => await _indexOperation.IndexClearAsync());

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
            var indexResult = await Record.ExceptionAsync(async () => await _indexOperation.IndexClearAsync());

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
            var domainException = await Record.ExceptionAsync(async () => await _indexOperation.IndexClearAsync());

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
            domainException = await Record.ExceptionAsync(async () => await _indexOperation.IndexClearAsync());

            // Assert
            Assert.NotNull(domainException);
            Assert.Equal("Test Error", domainException.Message);
            Assert.NotNull(domainException.InnerException);
        }

        #endregion Index Clear Tests
    }
}