using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Azure.Search.Documents.Models;
using Cogworks.AzureSearch.Exceptions.DocumentsExceptions;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.UnitTests.Models;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace Cogworks.AzureSearch.UnitTests.Operations
{
    public class DocumentOperationTests : TestBase
    {
        private readonly IDocumentOperation<TestDocumentModel> _documentOperation;

        public DocumentOperationTests()
            => _documentOperation = new Repository<TestDocumentModel>(
                IndexOperation,
                DocumentOperation,
                Search);

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public async Task Should_AddOrUpdateDocuments(int documentsCount)
        {
            // Arrange
            var testDocuments = Enumerable.Range(0, documentsCount)
                .Select(_ => Fixture.Create<TestDocumentModel>())
                .ToArray();


            var indexResults = testDocuments
                .Select(testDocument => SearchModelFactory.IndexingResult(
                    key: testDocument.Id,
                    errorMessage: string.Empty,
                    succeeded: true,
                    status: 200))
                .ToList();

            var documentIndexResult = SearchModelFactory.IndexDocumentsResult(indexResults);

            _ = DocumentOperationWrapper
                .IndexAsync(Arg.Any<IndexDocumentsBatch<TestDocumentModel>>())
                .Returns(new TestResponse<IndexDocumentsResult>(documentIndexResult));

            // Act
            var result = await _documentOperation.AddOrUpdateDocumentsAsync(testDocuments);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.NotEmpty(result.SucceededDocuments);
            Assert.Empty(result.FailedDocuments);
            Assert.All(
                result.SucceededDocuments,
                succeedItem =>
                {
                    Assert.True(succeedItem.Succeeded);
                    Assert.Equal(200, succeedItem.StatusCode);
                    Assert.Equal("Successfully adding or updating document.", succeedItem.Message);
                    Assert.Contains(testDocuments, item => item.Id == succeedItem.ModelId);
                });
        }

        [Theory]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public async Task Should_Fail_When_AddOrUpdateDocumentsPartiallyFail(int documentsCount)
        {
            // Arrange
            var testDocuments = Enumerable.Range(0, documentsCount)
                .Select(_ => Fixture.Create<TestDocumentModel>())
                .ToArray();


            var indexResults = testDocuments
                .Select((document, index) => new
                {
                    Document = document,
                    Succeded = index % (documentsCount / 2) == 0
                })
                .Select(item => SearchModelFactory.IndexingResult(
                    key: item.Document.Id,
                    errorMessage: !item.Succeded
                        ? "Internal Unit Error."
                        : string.Empty,
                    succeeded: item.Succeded,
                    status: item.Succeded
                        ? 200
                        : 404))
                .ToList();

            var documentIndexResult = SearchModelFactory.IndexDocumentsResult(indexResults);

            _ = DocumentOperationWrapper
                .IndexAsync(Arg.Any<IndexDocumentsBatch<TestDocumentModel>>())
                .Returns(new TestResponse<IndexDocumentsResult>(documentIndexResult));

            // Act
            var result = await _documentOperation.AddOrUpdateDocumentsAsync(testDocuments);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.NotEmpty(result.SucceededDocuments);
            Assert.NotEmpty(result.FailedDocuments);
            Assert.All(
                result.SucceededDocuments,
                succeedItem =>
                {
                    Assert.True(succeedItem.Succeeded);
                    Assert.Equal(200, succeedItem.StatusCode);
                    Assert.Equal("Successfully adding or updating document.", succeedItem.Message);
                    Assert.Contains(testDocuments, item => item.Id == succeedItem.ModelId);
                });

            Assert.All(
                result.FailedDocuments,
                succeedItem =>
                {
                    Assert.False(succeedItem.Succeeded);
                    Assert.Equal(404, succeedItem.StatusCode);
                    Assert.Equal("Failed adding or updating document.", succeedItem.Message);
                    Assert.Equal("Internal Unit Error.", succeedItem.InnerMessage);
                    Assert.Contains(testDocuments, item => item.Id == succeedItem.ModelId);
                });
        }

        [Fact]
        public async Task Should_Succeeded_When_TryToAddEmptyDocuments()
        {
            // Arrange
            var testDocuments = Enumerable.Empty<TestDocumentModel>();

            var documentIndexResult = new TestResponse<IndexDocumentsResult>(
                SearchModelFactory.IndexDocumentsResult(
                    Enumerable.Empty<IndexingResult>()));

            _ = DocumentOperationWrapper
                .IndexAsync(Arg.Any<IndexDocumentsBatch<TestDocumentModel>>())
                .Returns(documentIndexResult);

            // Act
            var result = await _documentOperation.AddOrUpdateDocumentsAsync(testDocuments);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Empty(result.SucceededDocuments);
            Assert.Empty(result.FailedDocuments);
            Assert.Equal("No documents found to index.", result.Message);
        }

        [Fact]
        public async Task Should_Succeeded_When_TryToRemoveEmptyDocuments()
        {
            // Arrange
            var testDocuments = Enumerable.Empty<TestDocumentModel>();

            var documentIndexResult = new TestResponse<IndexDocumentsResult>(
                SearchModelFactory.IndexDocumentsResult(
                    Enumerable.Empty<IndexingResult>()));

            _ = DocumentOperationWrapper
                .IndexAsync(Arg.Any<IndexDocumentsBatch<TestDocumentModel>>())
                .Returns(documentIndexResult);

            // Act
            var result = await _documentOperation.TryRemoveDocumentsAsync(testDocuments);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.Empty(result.SucceededDocuments);
            Assert.Empty(result.FailedDocuments);
            Assert.Equal("No documents found to delete.", result.Message);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public async Task Should_Succeeded_When_TryRemoveDocuments(int documentsCount)
        {
            // Arrange
            var testDocuments = Enumerable.Range(0, documentsCount)
                .Select(_ => Fixture.Create<TestDocumentModel>())
                .ToArray();

            var indexResults = testDocuments
                .Select(testDocument => SearchModelFactory.IndexingResult(
                    key: testDocument.Id,
                    errorMessage: string.Empty,
                    succeeded: true,
                    status: 200))
                .ToList();

            var documentIndexResult = SearchModelFactory.IndexDocumentsResult(indexResults);

            _ = DocumentOperationWrapper
                .IndexAsync(Arg.Any<IndexDocumentsBatch<TestDocumentModel>>())
                .Returns(new TestResponse<IndexDocumentsResult>(documentIndexResult));

            // Act
            var result = await _documentOperation.TryRemoveDocumentsAsync(testDocuments);

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Succeeded);
            Assert.NotEmpty(result.SucceededDocuments);
            Assert.Empty(result.FailedDocuments);
            Assert.All(
                result.SucceededDocuments,
                succeedItem =>
                {
                    Assert.True(succeedItem.Succeeded);
                    Assert.Equal(200, succeedItem.StatusCode);
                    Assert.Equal("Successfully removing document.", succeedItem.Message);
                    Assert.Contains(testDocuments, item => item.Id == succeedItem.ModelId);
                });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public async Task Should_Fail_When_TryRemoveNotExistingDocuments(int documentsCount)
        {
            // Arrange
            var testDocuments = Enumerable.Range(0, documentsCount)
                .Select(_ => Fixture.Create<TestDocumentModel>())
                .ToArray();

            var indexResults = testDocuments
                .Select(testDocument => SearchModelFactory.IndexingResult(
                    key: testDocument.Id,
                    errorMessage: string.Empty,
                    succeeded: false,
                    status: 404))
                .ToList();

            var documentIndexResult = SearchModelFactory.IndexDocumentsResult(indexResults);

            _ = DocumentOperationWrapper
                .IndexAsync(Arg.Any<IndexDocumentsBatch<TestDocumentModel>>())
                .Returns(new TestResponse<IndexDocumentsResult>(documentIndexResult));

            // Act
            var result = await _documentOperation.TryRemoveDocumentsAsync(testDocuments);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Succeeded);
            Assert.Empty(result.SucceededDocuments);
            Assert.NotEmpty(result.FailedDocuments);
            Assert.All(
                result.FailedDocuments,
                failedItem =>
                {
                    Assert.False(failedItem.Succeeded);
                    Assert.Equal(404, failedItem.StatusCode);
                    Assert.Equal("Failed removing document.", failedItem.Message);
                    Assert.Contains(testDocuments, item => item.Id == failedItem.ModelId);
                });
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public async Task Should_ThrowDomainException_When_IssueWith_TryAddOrUpdateDocuments(int documentsCount)
        {
            // Arrange

            var testDocuments = Enumerable.Range(0, documentsCount)
                .Select(_ => Fixture.Create<TestDocumentModel>())
                .ToArray();

            _ = DocumentOperationWrapper
                .IndexAsync(Arg.Any<IndexDocumentsBatch<TestDocumentModel>>())
                .Throws(_ => new AddOrUpdateDocumentException("Test Error", Fixture.Create<Exception>()));

            // Assert
            var domainException = await Assert.ThrowsAsync<AddOrUpdateDocumentException>(async () =>
                await _documentOperation.AddOrUpdateDocumentsAsync(testDocuments));

            Assert.NotNull(domainException);
            Assert.Equal("Test Error", domainException.Message);
            Assert.NotNull(domainException.InnerException);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public async Task Should_Not_ThrowDomainException_When_No_IssueWith_TryAddOrUpdateDocuments(int documentsCount)
        {
            // Arrange
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(true);

            // Act
            var domainExceptionResult = await Record.ExceptionAsync(() => Should_AddOrUpdateDocuments(documentsCount));

            // Assert
            Assert.Null(domainExceptionResult);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public async Task Should_ThrowDomainException_When_IssueWith_TryRemoveDocuments(int documentsCount)
        {
            // Arrange

            var testDocuments = Enumerable.Range(0, documentsCount)
                .Select(_ => Fixture.Create<TestDocumentModel>())
                .ToArray();

            _ = DocumentOperationWrapper
                .IndexAsync(Arg.Any<IndexDocumentsBatch<TestDocumentModel>>())
                .Throws(_ => new AddOrUpdateDocumentException("Test Error", Fixture.Create<Exception>()));

            // Assert
            var domainException = await Assert.ThrowsAsync<RemoveDocumentException>(async () =>
                await _documentOperation.TryRemoveDocumentsAsync(testDocuments));

            Assert.NotNull(domainException);
            Assert.Equal("Test Error", domainException.Message);
            Assert.NotNull(domainException.InnerException);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(1000)]
        public async Task Should_Not_ThrowDomainException_When_No_IssueWith_TryRemoveDocuments(int documentsCount)
        {
            // Arrange
            _ = IndexOperationWrapper.ExistsAsync(Arg.Any<string>())
                .Returns(true);

            // Act
            var domainExceptionResult = await Record.ExceptionAsync(() => Should_Succeeded_When_TryRemoveDocuments(documentsCount));

            // Assert
            Assert.Null(domainExceptionResult);
        }
    }
}