// ReSharper disable PossibleNullReferenceException

using System;
using System.IO;
using System.Reflection;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.Interfaces.Builder;
using Cogworks.AzureSearch.Interfaces.Indexes;
using Cogworks.AzureSearch.Interfaces.Initializers;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Repositories;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.IoC.Umbraco.Extensions;
using Cogworks.AzureSearch.IoC.Umbraco.UnitTests.Models;
using Cogworks.AzureSearch.IoC.Umbraco.UnitTests.Searchers;
using Cogworks.AzureSearch.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Logging;
using Xunit;

namespace Cogworks.AzureSearch.IoC.Umbraco.UnitTests
{
    public class UmbracoIocExtensionTests
    {
        private readonly IContainerBuilder _containerBuilder;
        private readonly ServiceCollection _serviceCollection;
        private readonly UmbracoBuilder _umbracoBuilder;

        private const string FirstDocumentIndexName = "first-test-document";
        private const string SecondDocumentIndexName = "second-test-document";
        private const string ThirdDocumentIndexName = "third-test-document";

        public UmbracoIocExtensionTests()
        {
            _serviceCollection = new ServiceCollection();
            var dirName = Path.GetDirectoryName(
                Assembly.GetExecutingAssembly()
                    .Location
                    .Replace("bin\\Debug", string.Empty));

            var typeLoader = new TypeLoader(
                Substitute.For<ITypeFinder>(),
                new VaryingRuntimeHash(),
                Substitute.For<IAppPolicyCache>(),
                new DirectoryInfo(dirName),
                Substitute.For<ILogger<TypeLoader>>(),
                Substitute.For<IProfiler>());

            _umbracoBuilder = new UmbracoBuilder(
                _serviceCollection,
                Substitute.For<IConfiguration>(),
                typeLoader);

            _containerBuilder = _umbracoBuilder.RegisterAzureSearch()
                .RegisterClientOptions("test", "test", "https://localhost")
                .RegisterIndexOptions(false, false)
                .RegisterIndexDefinitions<FirstTestDocumentModel>(FirstDocumentIndexName)
                .RegisterIndexDefinitions<SecondTestDocumentModel>(SecondDocumentIndexName)
                .RegisterIndexDefinitions<ThirdTestDocumentModel>(customIndex: new SearchIndex(ThirdDocumentIndexName));
        }

        [Theory]
        [InlineData(typeof(IRepository<FirstTestDocumentModel>))]
        [InlineData(typeof(IIndexOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IDocumentOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IIndex<FirstTestDocumentModel>))]
        [InlineData(typeof(IInitializer<FirstTestDocumentModel>))]
        [InlineData(typeof(ISearcher<FirstTestDocumentModel>))]
        [InlineData(typeof(IRepository<SecondTestDocumentModel>))]
        [InlineData(typeof(IIndexOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IDocumentOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IIndex<SecondTestDocumentModel>))]
        [InlineData(typeof(IInitializer<SecondTestDocumentModel>))]
        [InlineData(typeof(ISearcher<SecondTestDocumentModel>))]
        [InlineData(typeof(IRepository<ThirdTestDocumentModel>))]
        [InlineData(typeof(IIndexOperation<ThirdTestDocumentModel>))]
        [InlineData(typeof(IDocumentOperation<ThirdTestDocumentModel>))]
        [InlineData(typeof(IIndex<ThirdTestDocumentModel>))]
        [InlineData(typeof(IInitializer<ThirdTestDocumentModel>))]
        [InlineData(typeof(ISearcher<ThirdTestDocumentModel>))]
        public void Should_ReturnDedicatedRepositoryInstance(Type desiredObjectType)
        {
            // Arrange
            _umbracoBuilder.Build();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                // Act
                var instance = scope.ServiceProvider.GetService(desiredObjectType);

                // Assert
                Assert.NotNull(instance);
                Assert.True(desiredObjectType.IsInstanceOfType(instance));
                Assert.NotEmpty(instance.GetType().GenericTypeArguments);
                Assert.Equal(desiredObjectType.GenericTypeArguments[0], instance.GetType().GenericTypeArguments[0]);
            }
        }

        [Theory]
        [InlineData(typeof(IRepository<FirstTestDocumentModel>))]
        [InlineData(typeof(IIndexOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IDocumentOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IIndex<FirstTestDocumentModel>))]
        [InlineData(typeof(IInitializer<FirstTestDocumentModel>))]
        [InlineData(typeof(ISearcher<FirstTestDocumentModel>))]
        [InlineData(typeof(IRepository<SecondTestDocumentModel>))]
        [InlineData(typeof(IIndexOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IDocumentOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IIndex<SecondTestDocumentModel>))]
        [InlineData(typeof(IInitializer<SecondTestDocumentModel>))]
        [InlineData(typeof(ISearcher<SecondTestDocumentModel>))]
        [InlineData(typeof(IRepository<ThirdTestDocumentModel>))]
        [InlineData(typeof(IIndexOperation<ThirdTestDocumentModel>))]
        [InlineData(typeof(IDocumentOperation<ThirdTestDocumentModel>))]
        [InlineData(typeof(IIndex<ThirdTestDocumentModel>))]
        [InlineData(typeof(IInitializer<ThirdTestDocumentModel>))]
        [InlineData(typeof(ISearcher<ThirdTestDocumentModel>))]
        public void Should_Not_ThrowException_When_IndexRegistered(Type desiredObjectType)
        {
            // Arrange
            // Act
            var exceptionRecord = Record.Exception(() => Should_ReturnDedicatedRepositoryInstance(desiredObjectType));

            // Assert
            Assert.Null(exceptionRecord);
        }

        [Theory]
        [InlineData(typeof(IRepository<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(IIndexOperation<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(IDocumentOperation<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(IIndex<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(IInitializer<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(ISearcher<NotRegisteredTestDocumentModel>))]
        public void Should_ThrowException_When_IndexNotRegistered(Type desiredObjectType)
        {
            // Arrange
            // Act
            var exceptionRecord = Record.Exception(() =>
            {
                _umbracoBuilder.Build();
                var serviceProvider = _serviceCollection.BuildServiceProvider();

                using (var scope = serviceProvider.CreateScope())
                {
                    // Act
                    _ = scope.ServiceProvider.GetService(desiredObjectType);
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
            _containerBuilder.RegisterDomainSearcher<CustomTestSearch, CustomTestSearch, FirstTestDocumentModel>();

            _umbracoBuilder.Build();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var customTestSearch = scope.ServiceProvider.GetService<CustomTestSearch>();

                // Assert
                Assert.NotNull(customTestSearch);
            }
        }

        [Fact]
        public void Should_InvokeCustomSearchServiceDomainMethod()
        {
            // Arrange
            var mockedCustomTestSearch = Substitute.For<ICustomTestSearch>();

            _containerBuilder.RegisterDomainSearcher<CustomTestSearch, ICustomTestSearch, FirstTestDocumentModel>(
                mockedCustomTestSearch);

            _umbracoBuilder.Build();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var customTestSearch = scope.ServiceProvider.GetService<ICustomTestSearch>();

                // Act
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
            _umbracoBuilder.Build();
            var serviceProvider = _serviceCollection.BuildServiceProvider();

            // Act
            using (var scope = serviceProvider.CreateScope())
            {
                var provider = scope.ServiceProvider;
                var firstTestDocumentIndexDefinition =
                    provider.GetService<IndexDefinition<FirstTestDocumentModel>>();
                var secondTestDocumentIndexDefinition =
                    provider.GetService<IndexDefinition<SecondTestDocumentModel>>();
                var thirdTestDocumentIndexDefinition =
                    provider.GetService<IndexDefinition<ThirdTestDocumentModel>>();

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