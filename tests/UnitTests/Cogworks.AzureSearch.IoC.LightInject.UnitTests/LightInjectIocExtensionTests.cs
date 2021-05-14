﻿using System;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.Interfaces.Builder;
using Cogworks.AzureSearch.Interfaces.Indexes;
using Cogworks.AzureSearch.Interfaces.Initializers;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Repositories;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.IoC.LightInject.Extensions;
using Cogworks.AzureSearch.IoC.LightInject.UnitTests.Models;
using Cogworks.AzureSearch.IoC.LightInject.UnitTests.Searchers;
using Cogworks.AzureSearch.Models;
using LightInject;
using NSubstitute;
using Xunit;

namespace Cogworks.AzureSearch.IoC.LightInject.UnitTests
{
    public class LightInjectIocExtensionTests
    {
        private readonly IContainerBuilder _containerBuilder;
        private readonly IServiceContainer _container;

        private const string FirstDocumentIndexName = "first-test-document";
        private const string SecondDocumentIndexName = "second-test-document";
        private const string ThirdDocumentIndexName = "third-test-document";

        public LightInjectIocExtensionTests()
        {
            _container = new ServiceContainer();

            _containerBuilder = _container.RegisterAzureSearch()
                .RegisterClientOptions("test", "test", "https://localhost")
                .RegisterIndexOptions(false, false)
                .RegisterIndexDefinitions<FirstTestDocumentModel>(FirstDocumentIndexName)
                .RegisterIndexDefinitions<SecondTestDocumentModel>(SecondDocumentIndexName)
                .RegisterIndexDefinitions<ThirdTestDocumentModel>(customIndex: new SearchIndex(ThirdDocumentIndexName)); ;
        }

        [Theory]
        [InlineData(typeof(IRepository<FirstTestDocumentModel>))]
        [InlineData(typeof(IIndexOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IDocumentOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(ISearcher<FirstTestDocumentModel>))]
        [InlineData(typeof(IIndex<FirstTestDocumentModel>))]
        [InlineData(typeof(IInitializer<FirstTestDocumentModel>))]
        [InlineData(typeof(IRepository<SecondTestDocumentModel>))]
        [InlineData(typeof(IIndexOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IDocumentOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(ISearcher<SecondTestDocumentModel>))]
        [InlineData(typeof(IIndex<SecondTestDocumentModel>))]
        [InlineData(typeof(IInitializer<SecondTestDocumentModel>))]
        [InlineData(typeof(IRepository<ThirdTestDocumentModel>))]
        [InlineData(typeof(IIndexOperation<ThirdTestDocumentModel>))]
        [InlineData(typeof(IDocumentOperation<ThirdTestDocumentModel>))]
        [InlineData(typeof(IIndex<ThirdTestDocumentModel>))]
        [InlineData(typeof(IInitializer<ThirdTestDocumentModel>))]
        [InlineData(typeof(ISearcher<ThirdTestDocumentModel>))]
        public void Should_ReturnDedicatedRepositoryInstance(Type desiredObjectType)
        {
            // Arrange

            // ReSharper disable once PossibleNullReferenceException
            using (var scope = _container.BeginScope())
            {
                // Act
                var instance = scope.GetInstance(desiredObjectType);

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
                // ReSharper disable once PossibleNullReferenceException
                using (var scope = _container.BeginScope())
                {
                    // Act
                    _ = scope.GetInstance(desiredObjectType);
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

            // ReSharper disable once PossibleNullReferenceException
            using (var scope = _container.BeginScope())
            {
                var customTestSearch = scope.GetInstance<CustomTestSearch>();

                // Assert
                Assert.NotNull(customTestSearch);
            }
        }

        [Fact]
        public void Should_InvokeCustomSearchServiceDomainMethod()
        {
            // Arrange
            var mockedCustomTestSearch = Substitute.For<ICustomTestSearch>();

            _containerBuilder.RegisterDomainSearcher<CustomTestSearch, ICustomTestSearch, FirstTestDocumentModel>(mockedCustomTestSearch);

            // ReSharper disable once PossibleNullReferenceException
            using (var scope = _container.BeginScope())
            {
                var customTestSearch = scope.GetInstance<ICustomTestSearch>();

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
            using (var scope = _container.BeginScope())
            {
                var firstTestDocumentIndexDefinition = scope.GetInstance<IndexDefinition<FirstTestDocumentModel>>();
                var secondTestDocumentIndexDefinition = scope.GetInstance<IndexDefinition<SecondTestDocumentModel>>();
                var thirdTestDocumentIndexDefinition = scope.GetInstance<IndexDefinition<ThirdTestDocumentModel>>();

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