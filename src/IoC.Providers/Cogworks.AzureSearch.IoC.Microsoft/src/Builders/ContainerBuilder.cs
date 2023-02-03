﻿using Azure.Search.Documents.Indexes.Models;
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
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cogworks.AzureSearch.IoC.Microsoft.Builders
{
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public ContainerBuilder(IServiceCollection serviceCollection)
            => _serviceCollection = serviceCollection;

        internal ContainerBuilder RegisterInitializers()
        {
            _serviceCollection.TryAddScoped(typeof(IInitializer<>), typeof(Initializer<>));

            return this;
        }

        public IContainerBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false)
        {
            _serviceCollection.TryAddSingleton(_ => new IndexOption(recreate, recreateOnUpdateFailure));

            return this;
        }

        public IContainerBuilder RegisterClientOptions(string serviceName, string credentials,
            string serviceEndpointUrl, bool searchHeaders = false)
        {
            _serviceCollection.TryAddSingleton(_ => new ClientOption(
                serviceName,
                credentials,
                serviceEndpointUrl));

            return this;
        }

        public IContainerBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IModel, new()
        {
            _serviceCollection.TryAddSingleton(_ => new IndexDefinition<TDocument>(indexName));

            return this;
        }

        public IContainerBuilder RegisterIndexDefinitions<TDocument>(SearchIndex customIndex)
            where TDocument : class, IModel, new()
        {
            _serviceCollection.TryAddSingleton(_ => new IndexDefinition<TDocument>(customIndex));

            return this;
        }

        internal ContainerBuilder RegisterIndexes()
        {
            _serviceCollection.TryAddScoped(typeof(IIndex<>), typeof(Index<>));

            return this;
        }

        internal ContainerBuilder RegisterWrappers()
        {
            _serviceCollection.TryAddScoped(typeof(IDocumentOperationWrapper<>), typeof(DocumentOperationWrapper<>));

            _serviceCollection.TryAddScoped<IIndexOperationWrapper, IndexOperationWrapper>();

            return this;
        }

        internal ContainerBuilder RegisterRepositories()
        {
            _serviceCollection.TryAddScoped(
                typeof(IRepository<>),
                typeof(Repository<>));

            return this;
        }

        internal ContainerBuilder RegisterSearchers()
        {
            _serviceCollection.TryAddScoped(typeof(ISearcher<>), typeof(Searcher<>));

            return this;
        }

        internal ContainerBuilder RegisterOperations()
        {
            _serviceCollection.TryAddScoped(typeof(IDocumentOperation<>), typeof(DocumentOperation<>));

            _serviceCollection.TryAddScoped(typeof(IIndexOperation<>), typeof(IndexOperation<>));

            return this;
        }

        public IContainerBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _serviceCollection.TryAddSingleton<TSearcherType, TSearcher>();

            return this;
        }

        public IContainerBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(TSearcherType instance)
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _serviceCollection.TryAddSingleton(instance);

            return this;
        }
    }
}