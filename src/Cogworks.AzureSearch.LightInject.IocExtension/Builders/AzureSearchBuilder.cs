﻿using Cogworks.AzureSearch.Builder;
using Cogworks.AzureSearch.Indexes;
using Cogworks.AzureSearch.Initializers;
using Cogworks.AzureSearch.Interfaces.Indexes;
using Cogworks.AzureSearch.Interfaces.Initializers;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Repositories;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.Searchers;
using Cogworks.AzureSearch.Wrappers;
using LightInject;

namespace Cogworks.AzureSearch.LightInject.IocExtension.Builders
{
    public class AzureSearchBuilder : IAzureSearchBuilder
    {
        private readonly IServiceContainer _container;

        public AzureSearchBuilder(IServiceContainer serviceContainer)
            => _container = serviceContainer;

        internal AzureSearchBuilder RegisterInitializers()
        {
            _ = _container.Register(
                typeof(IAzureInitializer<>),
                typeof(AzureInitializer<>));

            return this;
        }

        public IAzureSearchBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false)
        {
            _ = _container.Register(
                _ => new AzureSearchIndexOption(recreate, recreateOnUpdateFailure),
                new PerContainerLifetime());

            return this;
        }

        public IAzureSearchBuilder RegisterClientOptions(string serviceName, string credentials)
        {
            _ = _container.Register(
                _ => new AzureSearchClientOption(serviceName, credentials),
                new PerContainerLifetime());

            return this;
        }

        public IAzureSearchBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IAzureModel, new()
        {
            _ = _container.Register(
                _ => new AzureIndexDefinition<TDocument>(indexName),
                new PerContainerLifetime());

            return this;
        }

        internal AzureSearchBuilder RegisterIndexes()
        {
            _ = _container.Register(
                typeof(IAzureIndex<>),
                typeof(AzureIndex<>));

            return this;
        }

        internal AzureSearchBuilder RegisterWrappers()
        {
            _ = _container.Register(
                typeof(IDocumentOperationWrapper<>),
                typeof(DocumentOperationWrapper<>));

            _ = _container.Register<IIndexOperationWrapper, IndexOperationWrapper>();

            return this;
        }

        internal AzureSearchBuilder RegisterRepositories()
        {
            _ = _container.Register(
                typeof(IAzureSearchRepository<>),
                typeof(AzureSearchRepository<>));

            _ = _container.Register(
                typeof(IAzureIndexOperation<>),
                typeof(AzureSearchRepository<>));

            _ = _container.Register(
                typeof(IAzureDocumentOperation<>),
                typeof(AzureSearchRepository<>));

            _ = _container.Register(
                typeof(IAzureDocumentSearch<>),
                typeof(AzureSearchRepository<>));

            return this;
        }

        internal AzureSearchBuilder RegisterSearchers()
        {
            _ = _container.Register(
                typeof(IAzureSearch<>),
                typeof(AzureSearch<>));

            return this;
        }

        public IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IAzureModel, new()
            where TSearcher : class, IAzureSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _ = _container.Register(typeof(TSearcherType), typeof(TSearcher), new PerContainerLifetime());

            return this;
        }

        public IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(TSearcherType instance)
            where TSearcher : class, IAzureSearch<TDocument>, TSearcherType
            where TSearcherType : class
            where TDocument : class, IAzureModel, new()
        {
            _ = _container.RegisterInstance(instance); ;

            return this;
        }
    }
}