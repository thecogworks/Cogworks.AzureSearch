using AutoFixture;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.UnitTests.Models;
using Microsoft.Azure.Search.Models;
using NSubstitute;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cogworks.AzureSearch.UnitTests.Repositories
{
    public class RepositoryDocumentOperationTests : RepositoryTestBase
    {
        private readonly IAzureDocumentOperation<TestDocumentModel> _azureDocumentOperation;

        public RepositoryDocumentOperationTests()
            => _azureDocumentOperation = new AzureSearchRepository<TestDocumentModel>(
                TestDocumentModelDefinition,
                IndexOperationWrapper,
                DocumentOperationWrapper);

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
                .Select(testDocument => new IndexingResult(testDocument.Id, succeeded: true, statusCode: 200))
                .ToList();

            var documentIndexResult = new DocumentIndexResult(indexResults);

            _ = DocumentOperationWrapper.IndexAsync(Arg.Any<IndexBatch<TestDocumentModel>>())
                .Returns(documentIndexResult);

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

            var documentIndexResult = new DocumentIndexResult();

            _ = DocumentOperationWrapper.IndexAsync(Arg.Any<IndexBatch<TestDocumentModel>>())
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

            var documentIndexResult = new DocumentIndexResult();

            _ = DocumentOperationWrapper.IndexAsync(Arg.Any<IndexBatch<TestDocumentModel>>())
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
                .Select(testDocument => new IndexingResult(testDocument.Id, succeeded: true, statusCode: 200))
                .ToList();

            var documentIndexResult = new DocumentIndexResult(indexResults);

            _ = DocumentOperationWrapper.IndexAsync(Arg.Any<IndexBatch<TestDocumentModel>>())
                .Returns(documentIndexResult);

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
                .Select(testDocument => new IndexingResult(testDocument.Id, succeeded: false, statusCode: 404))
                .ToList();

            var documentIndexResult = new DocumentIndexResult(indexResults);

            _ = DocumentOperationWrapper.IndexAsync(Arg.Any<IndexBatch<TestDocumentModel>>())
                .Returns(documentIndexResult);

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