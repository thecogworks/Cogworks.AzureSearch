using AutofacContainerBuilder = Autofac.ContainerBuilder;
using Cogworks.AzureSearch.Interfaces.Indexes;
using Cogworks.AzureSearch.Interfaces.Initializers;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Repositories;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;
using NSubstitute;
using System;
using Autofac;
using Cogworks.AzureSearch.AutofacIoc.UnitTests.Models;
using Cogworks.AzureSearch.AutofacIoc.UnitTests.Searchers;
using Xunit;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.Interfaces.Builder;
using Cogworks.AzureSearch.IoC.Autofac.Extensions;

namespace Cogworks.AzureSearch.AutofacIoc.UnitTests
{
    public class AutofacIocExtensionTests
    {
        private readonly IContainerBuilder _containerBuilder;
        private readonly AutofacContainerBuilder _autofacContainerBuilder;

        private const string FirstDocumentIndexName = "first-test-document";
        private const string SecondDocumentIndexName = "second-test-document";
        private const string ThirdDocumentIndexName = "third-test-document";

        public AutofacIocExtensionTests()
        {
            _autofacContainerBuilder = new AutofacContainerBuilder();

            _containerBuilder = _autofacContainerBuilder.RegisterAzureSearch()
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
        [InlineData(typeof(ISearcher<ThirdTestDocumentModel>))]
        [InlineData(typeof(IIndex<ThirdTestDocumentModel>))]
        [InlineData(typeof(IInitializer<ThirdTestDocumentModel>))]
        public void Should_ReturnDedicatedRepositoryInstance(Type desiredObjectType)
        {
            // Arrange

            // ReSharper disable once PossibleNullReferenceException
            using (var scope = _autofacContainerBuilder.Build().BeginLifetimeScope().BeginLifetimeScope())
            {
                // Act
                var instance = scope.Resolve(desiredObjectType);

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
            //            NotRegisteredTestDocumentModel
            // Arrange

            // Act
            var exceptionRecord = Record.Exception(() =>
            {
                // ReSharper disable once PossibleNullReferenceException
                using (var scope = _autofacContainerBuilder.Build().BeginLifetimeScope())
                {
                    // Act
                    _ = scope.Resolve(desiredObjectType);
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
            using (var scope = _autofacContainerBuilder.Build().BeginLifetimeScope())
            {

                var customTestSearch = scope.Resolve<CustomTestSearch>();

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
            using (var scope = _autofacContainerBuilder.Build().BeginLifetimeScope())
            {
                var customTestSearch = scope.Resolve<ICustomTestSearch>();

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
            using (var scope = _autofacContainerBuilder.Build().BeginLifetimeScope())
            {
                var firstTestDocumentIndexDefinition = scope.Resolve<IndexDefinition<FirstTestDocumentModel>>();
                var secondTestDocumentIndexDefinition = scope.Resolve<IndexDefinition<SecondTestDocumentModel>>();
                var thirdTestDocumentIndexDefinition = scope.Resolve<IndexDefinition<ThirdTestDocumentModel>>();

                // Assert
                Assert.NotNull(firstTestDocumentIndexDefinition);
                Assert.NotNull(secondTestDocumentIndexDefinition);
                Assert.Equal(FirstDocumentIndexName, firstTestDocumentIndexDefinition.IndexName);
                Assert.Equal(SecondDocumentIndexName, secondTestDocumentIndexDefinition.IndexName);
                Assert.Equal(ThirdDocumentIndexName, thirdTestDocumentIndexDefinition.IndexName);
            }
        }
    }
}