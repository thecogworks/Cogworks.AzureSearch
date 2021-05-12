using Cogworks.AzureSearch.Builder;
using Cogworks.AzureSearch.Interfaces.Indexes;
using Cogworks.AzureSearch.Interfaces.Initializers;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Repositories;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Microsoft.IocExtension.Extensions;
using Cogworks.AzureSearch.Models;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using System;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.MicrosoftIoc.UnitTests.Models;
using Cogworks.AzureSearch.MicrosoftIoc.UnitTests.Searchers;
using Xunit;

namespace Cogworks.AzureSearch.MicrosoftIoc.UnitTests
{
    public class MicrosoftIocExtensionTests
    {
        private readonly IAzureSearchBuilder _azureSearchBuilder;
        private readonly IServiceCollection _serviceContainer;

        private const string FirstDocumentIndexName = "first-test-document";
        private const string SecondDocumentIndexName = "second-test-document";
        private const string ThirdDocumentIndexName = "third-test-document";

        public MicrosoftIocExtensionTests()
        {
            _serviceContainer = new ServiceCollection();

            _azureSearchBuilder = _serviceContainer.RegisterAzureSearch()
                .RegisterClientOptions("test", "test", "https://localhost")
                .RegisterIndexOptions(false, false)
                .RegisterIndexDefinitions<FirstTestDocumentModel>(FirstDocumentIndexName)
                .RegisterIndexDefinitions<SecondTestDocumentModel>(SecondDocumentIndexName)
                .RegisterIndexDefinitions<ThirdTestDocumentModel>(customIndex: new SearchIndex(ThirdDocumentIndexName));
        }

        [Theory]
        [InlineData(typeof(IAzureSearchRepository<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureIndexOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureIndex<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureInitializer<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureSearch<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureSearchRepository<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureIndexOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureIndex<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureInitializer<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureSearch<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureSearchRepository<ThirdTestDocumentModel>))]
        [InlineData(typeof(IAzureIndexOperation<ThirdTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentOperation<ThirdTestDocumentModel>))]
        [InlineData(typeof(IAzureIndex<ThirdTestDocumentModel>))]
        [InlineData(typeof(IAzureInitializer<ThirdTestDocumentModel>))]
        [InlineData(typeof(IAzureSearch<ThirdTestDocumentModel>))]
        public void Should_ReturnDedicatedRepositoryInstance(Type desiredObjectType)
        {
            // Arrange

            // ReSharper disable once PossibleNullReferenceException
            using (var serviceProvider = _serviceContainer.BuildServiceProvider())
            {
                // Act
                var instance = serviceProvider.GetService(desiredObjectType);

                // Assert
                Assert.NotNull(instance);
                Assert.True(desiredObjectType.IsInstanceOfType(instance));
                Assert.NotEmpty(instance.GetType().GenericTypeArguments);
                Assert.Equal(desiredObjectType.GenericTypeArguments[0], instance.GetType().GenericTypeArguments[0]);
            }
        }

        [Theory]
        [InlineData(typeof(IAzureSearchRepository<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureIndexOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureIndex<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureInitializer<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureSearch<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureSearchRepository<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureIndexOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureIndex<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureInitializer<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureSearch<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureSearchRepository<ThirdTestDocumentModel>))]
        [InlineData(typeof(IAzureIndexOperation<ThirdTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentOperation<ThirdTestDocumentModel>))]
        [InlineData(typeof(IAzureIndex<ThirdTestDocumentModel>))]
        [InlineData(typeof(IAzureInitializer<ThirdTestDocumentModel>))]
        [InlineData(typeof(IAzureSearch<ThirdTestDocumentModel>))]
        public void Should_Not_ThrowException_When_IndexRegistered(Type desiredObjectType)
        {
            // Arrange
            // Act
            var exceptionRecord = Record.Exception(() => Should_ReturnDedicatedRepositoryInstance(desiredObjectType));

            // Assert
            Assert.Null(exceptionRecord);
        }

        [Theory]
        [InlineData(typeof(IAzureSearchRepository<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(IAzureIndexOperation<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentOperation<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(IAzureIndex<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(IAzureInitializer<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(IAzureSearch<NotRegisteredTestDocumentModel>))]
        public void Should_ThrowException_When_IndexNotRegistered(Type desiredObjectType)
        {
            //            NotRegisteredTestDocumentModel
            // Arrange

            // Act
            var exceptionRecord = Record.Exception(() =>
            {
                // ReSharper disable once PossibleNullReferenceException
                using (var serviceProvider = _serviceContainer.BuildServiceProvider())
                {
                    // Act
                    _ = serviceProvider.GetService(desiredObjectType);
                }
            });

            // Assert
            Assert.NotNull(exceptionRecord);
        }

        [Fact]
        public void Should_Not_ThrowException_When_GettingCustomSearchService()
        {
            // Arrange
            // Act
            var exceptionRecord = Record.Exception(Should_ReturnCustomSearchService);

            // Assert
            Assert.Null(exceptionRecord);
        }

        [Fact]
        public void Should_ReturnCustomSearchService()
        {
            // Arrange
            _azureSearchBuilder.RegisterDomainSearcher<CustomTestSearch, CustomTestSearch, FirstTestDocumentModel>();

            // ReSharper disable once PossibleNullReferenceException
            using (var serviceProvider = _serviceContainer.BuildServiceProvider())
            {
                var customTestSearch = serviceProvider.GetService<CustomTestSearch>();

                // Assert
                Assert.NotNull(customTestSearch);
            }
        }

        [Fact]
        public void Should_InvokeCustomSearchServiceDomainMethod()
        {
            // Arrange
            var mockedCustomTestSearch = Substitute.For<ICustomTestSearch>();

            _azureSearchBuilder.RegisterDomainSearcher<CustomTestSearch, ICustomTestSearch, FirstTestDocumentModel>(mockedCustomTestSearch);

            // ReSharper disable once PossibleNullReferenceException
            using (var serviceProvider = _serviceContainer.BuildServiceProvider())
            {
                var customTestSearch = serviceProvider.GetService<ICustomTestSearch>();

                customTestSearch.SomeCustomSearchExample();

                // Assert
                Assert.NotNull(customTestSearch);

                mockedCustomTestSearch.Received(1).SomeCustomSearchExample();

                customTestSearch.SomeCustomSearchExample();

                mockedCustomTestSearch.Received(2).SomeCustomSearchExample();
            }
        }

        [Fact]
        public void Should_Not_ThrowException_When_GettingDedicatedIndexWithProperName()
        {
            // Arrange
            // Act
            var exceptionRecord = Record.Exception(Should_GetDedicatedIndexWithProperName);

            // Assert
            Assert.Null(exceptionRecord);
        }

        [Fact]
        public void Should_GetDedicatedIndexWithProperName()
        {
            // Arrange
            // Act

            // ReSharper disable once PossibleNullReferenceException
            using (var serviceProvider = _serviceContainer.BuildServiceProvider())
            {
                var firstTestDocumentIndexDefinition = serviceProvider.GetService<AzureIndexDefinition<FirstTestDocumentModel>>();
                var secondTestDocumentIndexDefinition = serviceProvider.GetService<AzureIndexDefinition<SecondTestDocumentModel>>();
                var thirdTestDocumentIndexDefinition = serviceProvider.GetService<AzureIndexDefinition<ThirdTestDocumentModel>>();

                // Assert
                Assert.NotNull(firstTestDocumentIndexDefinition);
                Assert.NotNull(secondTestDocumentIndexDefinition);
                Assert.NotNull(thirdTestDocumentIndexDefinition);
                Assert.Equal(FirstDocumentIndexName, firstTestDocumentIndexDefinition.IndexName);
                Assert.Equal(SecondDocumentIndexName, secondTestDocumentIndexDefinition.IndexName);
                Assert.Equal(ThirdDocumentIndexName, thirdTestDocumentIndexDefinition.IndexName);
            }
        }
    }
}