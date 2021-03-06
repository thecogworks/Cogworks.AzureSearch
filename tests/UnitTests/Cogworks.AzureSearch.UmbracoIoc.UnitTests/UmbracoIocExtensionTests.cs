﻿// ReSharper disable PossibleNullReferenceException
using Cogworks.AzureSearch.Builder;
using Cogworks.AzureSearch.Interfaces.Indexes;
using Cogworks.AzureSearch.Interfaces.Initializers;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Repositories;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Umbraco.IocExtension.Extensions;
using Cogworks.AzureSearch.UmbracoIoc.UnitTests.Models;
using Cogworks.AzureSearch.UmbracoIoc.UnitTests.Searchers;
using LightInject;
using NSubstitute;
using System;
using Umbraco.Core.Composing;
using Umbraco.Core.Composing.LightInject;
using Xunit;

namespace Cogworks.AzureSearch.UmbracoIoc.UnitTests
{
    public class UmbracoIocExtensionTests
    {
        private readonly IAzureSearchBuilder _azureSearchBuilder;
        private readonly Composition _composing;

        private const string FirstDocumentIndexName = "first-test-document";
        private const string SecondDocumentIndexName = "second-test-document";

        public UmbracoIocExtensionTests()
        {
            var lightInjectContainer = LightInjectContainer.Create();

            _composing = new Composition(lightInjectContainer, null, null, null, null);

            _azureSearchBuilder = _composing.RegisterAzureSearch()
                .RegisterClientOptions("test", "test")
                .RegisterIndexOptions(false, false)
                .RegisterIndexDefinitions<FirstTestDocumentModel>(FirstDocumentIndexName)
                .RegisterIndexDefinitions<SecondTestDocumentModel>(SecondDocumentIndexName);
        }

        [Theory]
        [InlineData(typeof(IAzureSearchRepository<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureIndexOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentSearch<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureIndex<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureInitializer<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureSearch<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureSearchRepository<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureIndexOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentSearch<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureIndex<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureInitializer<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureSearch<SecondTestDocumentModel>))]
        public void Should_ReturnDedicatedRepositoryInstance(Type desiredObjectType)
        {
            // Arrange
            var container = _composing.Concrete as ServiceContainer as IServiceContainer;

            using (var scope = container.BeginScope())
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
        [InlineData(typeof(IAzureSearchRepository<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureIndexOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentOperation<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentSearch<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureIndex<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureInitializer<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureSearch<FirstTestDocumentModel>))]
        [InlineData(typeof(IAzureSearchRepository<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureIndexOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentOperation<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureDocumentSearch<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureIndex<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureInitializer<SecondTestDocumentModel>))]
        [InlineData(typeof(IAzureSearch<SecondTestDocumentModel>))]
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
        [InlineData(typeof(IAzureDocumentSearch<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(IAzureIndex<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(IAzureInitializer<NotRegisteredTestDocumentModel>))]
        [InlineData(typeof(IAzureSearch<NotRegisteredTestDocumentModel>))]
        public void Should_ThrowException_When_IndexNotRegistered(Type desiredObjectType)
        {
            // Arrange
            var container = _composing.Concrete as ServiceContainer as IServiceContainer;

            // Act
            var exceptionRecord = Record.Exception(() =>
            {
                using (var scope = container.BeginScope())
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
            _azureSearchBuilder.RegisterDomainSearcher<CustomTestSearch, CustomTestSearch, FirstTestDocumentModel>();

            var container = _composing.Concrete as ServiceContainer as IServiceContainer;

            using (var scope = container.BeginScope())
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

            _azureSearchBuilder.RegisterDomainSearcher<CustomTestSearch, ICustomTestSearch, FirstTestDocumentModel>(mockedCustomTestSearch);

            var container = _composing.Concrete as ServiceContainer as IServiceContainer;

            using (var scope = container.BeginScope())
            {
                var customTestSearch = scope.GetInstance<ICustomTestSearch>();

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
            // Act
            var container = _composing.Concrete as ServiceContainer as IServiceContainer;

            using (var scope = container.BeginScope())
            {
                var firstTestDocumentIndexDefinition = scope.GetInstance<AzureIndexDefinition<FirstTestDocumentModel>>();
                var secondTestDocumentIndexDefinition = scope.GetInstance<AzureIndexDefinition<SecondTestDocumentModel>>();

                // Assert
                Assert.NotNull(firstTestDocumentIndexDefinition);
                Assert.NotNull(secondTestDocumentIndexDefinition);
                Assert.Equal(FirstDocumentIndexName, firstTestDocumentIndexDefinition.IndexName);
                Assert.Equal(SecondDocumentIndexName, secondTestDocumentIndexDefinition.IndexName);
            }
        }
    }
}