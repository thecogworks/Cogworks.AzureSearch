using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Azure;
using Azure.Search.Documents.Models;
using Cogworks.AzureCognitiveSearch.Interfaces.Operations;
using Cogworks.AzureCognitiveSearch.Repositories;
using Cogworks.AzureCognitiveSearch.UnitTests.Models;
using NSubstitute;
using Xunit;

namespace Cogworks.AzureCognitiveSearch.UnitTests.Operations
{
    public class DocumentOperationTests : TestBase
    {
        private readonly IAzureDocumentOperation<TestDocumentModel> _azureDocumentOperation;

        public DocumentOperationTests()
            => _azureDocumentOperation = new AzureSearchRepository<TestDocumentModel>(
                AzureIndexOperationService,
                AzureDocumentOperationService,
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
            var result = await _azureDocumentOperation.AddOrUpdateDocumentsAsync(testDocuments);

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
            var result = await _azureDocumentOperation.AddOrUpdateDocumentsAsync(testDocuments);

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
            var result = await _azureDocumentOperation.TryRemoveDocumentsAsync(testDocuments);

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
            var result = await _azureDocumentOperation.TryRemoveDocumentsAsync(testDocuments);

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
            var result = await _azureDocumentOperation.TryRemoveDocumentsAsync(testDocuments);

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
    }
}