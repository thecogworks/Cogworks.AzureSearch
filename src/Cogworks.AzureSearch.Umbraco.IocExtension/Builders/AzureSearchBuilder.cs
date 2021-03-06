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
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Cogworks.AzureSearch.Umbraco.IocExtension.Builders
{
    public class AzureSearchBuilder : IAzureSearchBuilder
    {
        private readonly IRegister _composingRegister;

        public AzureSearchBuilder(IRegister composingRegister)
            => _composingRegister = composingRegister;

        internal AzureSearchBuilder RegisterInitializers()
        {
            _composingRegister.Register(
                typeof(IAzureInitializer<>),
                typeof(AzureInitializer<>));

            return this;
        }

        public IAzureSearchBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false)
        {
            _composingRegister.Register(
                _ => new AzureSearchIndexOption(recreate, recreateOnUpdateFailure),
                Lifetime.Singleton);

            return this;
        }

        public IAzureSearchBuilder RegisterClientOptions(string serviceName, string credentials)
        {
            _composingRegister.Register(
                _ => new AzureSearchClientOption(serviceName, credentials),
                Lifetime.Singleton);

            return this;
        }

        public IAzureSearchBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IAzureModel, new()
        {
            _composingRegister.Register(
                _ => new AzureIndexDefinition<TDocument>(indexName),
                Lifetime.Singleton);

            return this;
        }

        internal AzureSearchBuilder RegisterIndexes()
        {
            _composingRegister.Register(
                typeof(IAzureIndex<>),
                typeof(AzureIndex<>));

            return this;
        }

        internal AzureSearchBuilder RegisterWrappers()
        {
            _composingRegister.Register(
                typeof(IDocumentOperationWrapper<>),
                typeof(DocumentOperationWrapper<>));

            _composingRegister.Register<IIndexOperationWrapper, IndexOperationWrapper>();

            return this;
        }

        internal AzureSearchBuilder RegisterRepositories()
        {
            _composingRegister.Register(
                typeof(IAzureSearchRepository<>),
                typeof(AzureSearchRepository<>));

            _composingRegister.Register(
                typeof(IAzureIndexOperation<>),
                typeof(AzureSearchRepository<>));

            _composingRegister.Register(
                typeof(IAzureDocumentOperation<>),
                typeof(AzureSearchRepository<>));

            _composingRegister.Register(
                typeof(IAzureDocumentSearch<>),
                typeof(AzureSearchRepository<>));

            return this;
        }

        internal AzureSearchBuilder RegisterSearchers()
        {
            _composingRegister.Register(
                typeof(IAzureSearch<>),
                typeof(AzureSearch<>));

            return this;
        }

        public IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IAzureModel, new()
            where TSearcher : class, IAzureSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _composingRegister.Register<TSearcherType, TSearcher>(Lifetime.Singleton);

            return this;
        }

        public IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(TSearcherType instance)
            where TDocument : class, IAzureModel, new()
            where TSearcher : class, IAzureSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _composingRegister.Register<TSearcherType>(instance);

            return this;
        }
    }
}