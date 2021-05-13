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
using LightInject;

namespace Cogworks.AzureSearch.IoC.LightInject.Builders
{
    public class ContainerBuilder : IContainerBuilder
    {
        private readonly IServiceContainer _container;

        public ContainerBuilder(IServiceContainer serviceContainer)
            => _container = serviceContainer;

        internal ContainerBuilder RegisterInitializers()
        {
            _ = _container.Register(
                typeof(IInitializer<>),
                typeof(Initializer<>));

            return this;
        }

        public IContainerBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false)
        {
            _ = _container.Register(
                _ => new IndexOption(recreate, recreateOnUpdateFailure),
                new PerContainerLifetime());

            return this;
        }

        public IContainerBuilder RegisterClientOptions(string serviceName, string credentials, string serviceEndpointUrl)
        {
            _ = _container.Register(
                _ => new ClientOption(
                    serviceName,
                    credentials,
                    serviceEndpointUrl),
                new PerContainerLifetime());

            return this;
        }

        public IContainerBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IModel, new()
        {
            _ = _container.Register(
                _ => new IndexDefinition<TDocument>(indexName),
                new PerContainerLifetime());

            return this;
        }

        public IContainerBuilder RegisterIndexDefinitions<TDocument>(SearchIndex customIndex)
            where TDocument : class, IModel, new()
        {
            _ = _container.Register(
                _ => new IndexDefinition<TDocument>(customIndex),
                new PerContainerLifetime());

            return this;
        }

        internal ContainerBuilder RegisterIndexes()
        {
            _ = _container.Register(
                typeof(IIndex<>),
                typeof(Index<>));

            return this;
        }

        internal ContainerBuilder RegisterWrappers()
        {
            _ = _container.Register(
                typeof(IDocumentOperationWrapper<>),
                typeof(DocumentOperationWrapper<>));

            _ = _container.Register<IIndexOperationWrapper, IndexOperationWrapper>();

            return this;
        }

        internal ContainerBuilder RegisterRepositories()
        {
            _ = _container.Register(
                typeof(IRepository<>),
                typeof(Repository<>));

            return this;
        }

        internal ContainerBuilder RegisterSearchers()
        {
            _ = _container.Register(
                typeof(ISearcher<>),
                typeof(Searcher<>));

            return this;
        }
        internal ContainerBuilder RegisterOperations()
        {
            _ = _container.Register(
                typeof(IDocumentOperation<>),
                typeof(DocumentOperation<>),
                new PerContainerLifetime());

            _ = _container.Register(
                typeof(IIndexOperation<>),
                typeof(IndexOperation<>),
                new PerContainerLifetime());

            return this;
        }

        public IContainerBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _ = _container.Register(typeof(TSearcherType), typeof(TSearcher), new PerContainerLifetime());

            return this;
        }

        public IContainerBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(TSearcherType instance)
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _ = _container.RegisterInstance(instance); ;

            return this;
        }
    }
}