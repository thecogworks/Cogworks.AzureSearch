using System;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.Indexes;
using Cogworks.AzureSearch.Initializers;
using Cogworks.AzureSearch.Interfaces.Builder;
using Cogworks.AzureSearch.Interfaces.Indexes;
using Cogworks.AzureSearch.Interfaces.Initializers;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Repositories;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Operations;
using Cogworks.AzureSearch.Options;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.Searchers;
using Cogworks.AzureSearch.Wrappers;
using Microsoft.Extensions.DependencyInjection;
using Umbraco.Cms.Core.DependencyInjection;

namespace Cogworks.AzureSearch.IoC.Umbraco.Builders
{
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly IUmbracoBuilder _umbracoBuilder;

        public ContainerBuilder(IUmbracoBuilder umbracoBuilder)
            => _umbracoBuilder = umbracoBuilder
                                 ?? throw new ArgumentNullException(
                                     nameof(umbracoBuilder));

        public IContainerBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false)
        {
            _umbracoBuilder
                .Services
                .AddSingleton(_ => new IndexOption(recreate, recreateOnUpdateFailure));

            return this;
        }

        public IContainerBuilder RegisterClientOptions(string serviceName, string credentials,
            string serviceEndpointUrl, bool searchHeaders = false)
        {
            _umbracoBuilder
                .Services
                .AddSingleton(
                    _ => new ClientOption(
                        serviceName,
                        credentials,
                        serviceEndpointUrl,
                        searchHeaders));

            return this;
        }

        public IContainerBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IModel, new()
        {
            _umbracoBuilder
                .Services
                .AddSingleton(
                    _ => new IndexDefinition<TDocument>(indexName));

            return this;
        }

        public IContainerBuilder RegisterIndexDefinitions<TDocument>(SearchIndex customIndex)
            where TDocument : class, IModel, new()
        {
            _umbracoBuilder
                .Services
                .AddSingleton(
                    _ => new IndexDefinition<TDocument>(customIndex));

            return this;
        }

        public IContainerBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _umbracoBuilder
                .Services
                .AddSingleton<TSearcherType, TSearcher>();

            return this;
        }

        public IContainerBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(TSearcherType instance)
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _umbracoBuilder.Services.AddTransient<TSearcherType>(_ => instance);

            return this;
        }

        internal ContainerBuilder RegisterInitializers()
        {
            _umbracoBuilder
                .Services
                .AddTransient(typeof(IInitializer<>), typeof(Initializer<>));

            return this;
        }

        internal ContainerBuilder RegisterIndexes()
        {
            _umbracoBuilder
                .Services
                .AddTransient(
                    typeof(IIndex<>),
                    typeof(Index<>));

            return this;
        }

        internal ContainerBuilder RegisterWrappers()
        {
            _umbracoBuilder
                .Services
                .AddTransient(
                    typeof(IDocumentOperationWrapper<>),
                    typeof(DocumentOperationWrapper<>));

            _umbracoBuilder
                .Services
                .AddTransient<IIndexOperationWrapper, IndexOperationWrapper>();

            return this;
        }

        internal ContainerBuilder RegisterRepositories()
        {
            _umbracoBuilder
                .Services
                .AddTransient(
                    typeof(IRepository<>),
                    typeof(Repository<>));

            return this;
        }

        internal ContainerBuilder RegisterSearchers()
        {
            _umbracoBuilder
                .Services
                .AddTransient(
                    typeof(ISearcher<>),
                    typeof(Searcher<>));

            return this;
        }

        internal ContainerBuilder RegisterOperations()
        {
            _umbracoBuilder
                .Services
                .AddTransient(
                    typeof(IDocumentOperation<>),
                    typeof(DocumentOperation<>));

            _umbracoBuilder
                .Services
                .AddTransient(
                    typeof(IIndexOperation<>),
                    typeof(IndexOperation<>));

            return this;
        }
    }
}