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
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Cogworks.AzureSearch.Microsoft.IocExtension.Builders
{
    public class AzureSearchBuilder : IAzureSearchBuilder
    {
        private readonly IServiceCollection _serviceCollection;

        public AzureSearchBuilder(IServiceCollection serviceCollection)
            => _serviceCollection = serviceCollection;

        internal AzureSearchBuilder RegisterInitializers()
        {
            _serviceCollection.TryAddScoped(typeof(IAzureInitializer<>), typeof(AzureInitializer<>));

            return this;
        }

        public IAzureSearchBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false)
        {
            _serviceCollection.TryAddSingleton(_ => new AzureSearchIndexOption(recreate, recreateOnUpdateFailure));

            return this;
        }

        public IAzureSearchBuilder RegisterClientOptions(string serviceName, string credentials)
        {
            _serviceCollection.TryAddSingleton(_ => new AzureSearchClientOption(serviceName, credentials));

            return this;
        }

        public IAzureSearchBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IAzureModel, new()
        {
            _serviceCollection.TryAddSingleton(_ => new AzureIndexDefinition<TDocument>(indexName));

            return this;
        }

        internal AzureSearchBuilder RegisterIndexes()
        {
            _serviceCollection.TryAddScoped(typeof(IAzureIndex<>), typeof(AzureIndex<>));

            return this;
        }

        internal AzureSearchBuilder RegisterWrappers()
        {
            _serviceCollection.TryAddScoped(typeof(IDocumentOperationWrapper<>), typeof(DocumentOperationWrapper<>));

            _serviceCollection.TryAddScoped<IIndexOperationWrapper, IndexOperationWrapper>();

            return this;
        }

        internal AzureSearchBuilder RegisterRepositories()
        {
            _serviceCollection.TryAddScoped(typeof(IAzureSearchRepository<>), typeof(AzureSearchRepository<>));
            _serviceCollection.TryAddScoped(typeof(IAzureIndexOperation<>), typeof(AzureSearchRepository<>));
            _serviceCollection.TryAddScoped(typeof(IAzureDocumentOperation<>), typeof(AzureSearchRepository<>));
            _serviceCollection.TryAddScoped(typeof(IAzureDocumentSearch<>), typeof(AzureSearchRepository<>));

            return this;
        }

        internal AzureSearchBuilder RegisterSearchers()
        {
            _serviceCollection.TryAddScoped(typeof(IAzureSearch<>), typeof(AzureSearch<>));

            return this;
        }

        public IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IAzureModel, new()
            where TSearcher : class, IAzureSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _serviceCollection.TryAddSingleton<TSearcherType, TSearcher>();

            return this;
        }

        IAzureSearchBuilder IAzureSearchBuilder.RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(
            TSearcherType instance)
        {
            _serviceCollection.TryAddSingleton(instance);

            return this;
        }
    }
}