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
using Umbraco.Core;
using Umbraco.Core.Composing;

namespace Cogworks.AzureSearch.IoC.Umbraco.Builders
{
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly IRegister _composingRegister;

        public ContainerBuilder(IRegister composingRegister)
            => _composingRegister = composingRegister;

        internal ContainerBuilder RegisterInitializers()
        {
            _composingRegister.Register(
                typeof(IInitializer<>),
                typeof(Initializer<>));

            return this;
        }

        public IContainerBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false)
        {
            _composingRegister.Register(
                _ => new IndexOption(recreate, recreateOnUpdateFailure),
                Lifetime.Singleton);

            return this;
        }

        public IContainerBuilder RegisterClientOptions(string serviceName, string credentials, string serviceEndpointUrl)
        {
            _composingRegister.Register(
                _ => new ClientOption(
                    serviceName,
                    credentials,
                    serviceEndpointUrl),
                Lifetime.Singleton);

            return this;
        }

        public IContainerBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IModel, new()
        {
            _composingRegister.Register(
                _ => new IndexDefinition<TDocument>(indexName),
                Lifetime.Singleton);

            return this;
        }

        public IContainerBuilder RegisterIndexDefinitions<TDocument>(SearchIndex customIndex)
            where TDocument : class, IModel, new()
        {
            _composingRegister.Register(
                _ => new IndexDefinition<TDocument>(customIndex),
                Lifetime.Singleton);

            return this;
        }

        internal ContainerBuilder RegisterIndexes()
        {
            _composingRegister.Register(
                typeof(IIndex<>),
                typeof(Index<>));

            return this;
        }

        internal ContainerBuilder RegisterWrappers()
        {
            _composingRegister.Register(
                typeof(IDocumentOperationWrapper<>),
                typeof(DocumentOperationWrapper<>));

            _composingRegister.Register<IIndexOperationWrapper, IndexOperationWrapper>();

            return this;
        }

        internal ContainerBuilder RegisterRepositories()
        {
            _composingRegister.Register(
                typeof(IRepository<>),
                typeof(Repository<>));

            return this;
        }

        internal ContainerBuilder RegisterSearchers()
        {
            _composingRegister.Register(
                typeof(ISearcher<>),
                typeof(Searcher<>));

            return this;
        }

        internal ContainerBuilder RegisterOperations()
        {
            _composingRegister.Register(
                typeof(IDocumentOperation<>),
                typeof(DocumentOperation<>));

            _composingRegister.Register(
                typeof(IIndexOperation<>),
                typeof(IndexOperation<>));

            return this;
        }

        public IContainerBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _composingRegister.Register<TSearcherType, TSearcher>(Lifetime.Singleton);

            return this;
        }

        public IContainerBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(TSearcherType instance)
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _composingRegister.Register<TSearcherType>(instance);

            return this;
        }
    }
}