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
    public class AzureSearchBuilder
    {
        private readonly IServiceContainer _container;

        public AzureSearchBuilder(IServiceContainer serviceContainer)
            => _container = serviceContainer;

        internal AzureSearchBuilder RegisterInitializers()
        {
            _container.Register(
                typeof(IAzureInitializer<>),
                typeof(AzureInitializer<>));

            return this;
        }

        public AzureSearchBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false)
        {
            _container.Register(
                _ => new AzureSearchIndexOption(recreate, recreateOnUpdateFailure),
                new PerContainerLifetime());

            return this;
        }

        public AzureSearchBuilder RegisterClientOptions(string serviceName, string credentials)
        {
            _container.Register(
                _ => new AzureSearchClientOption(serviceName, credentials),
                new PerContainerLifetime());

            return this;
        }

        public AzureSearchBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IAzureModel, new()
        {
            _container.Register(
                _ => new AzureIndexDefinition<TDocument>(indexName),
                new PerContainerLifetime());

            return this;
        }

        internal AzureSearchBuilder RegisterIndexes()
        {
            _container.Register(
                typeof(IAzureIndex<>),
                typeof(AzureIndex<>));

            return this;
        }

        internal AzureSearchBuilder RegisterWrappers()
        {
            _container.Register(
                typeof(IDocumentOperationWrapper<>),
                typeof(DocumentOperationWrapper<>));

            _container.Register<IIndexOperationWrapper, IndexOperationWrapper>();

            return this;
        }

        internal AzureSearchBuilder RegisterRepositories()
        {
            _container.Register(
                typeof(IAzureSearchRepository<>),
                typeof(AzureSearchRepository<>));

            _container.Register(
                typeof(IAzureIndexOperation<>),
                typeof(AzureSearchRepository<>));

            _container.Register(
                typeof(IAzureDocumentOperation<>),
                typeof(AzureSearchRepository<>));

            _container.Register(
                typeof(IAzureDocumentSearch<>),
                typeof(AzureSearchRepository<>));

            return this;
        }

        internal AzureSearchBuilder RegisterSearchers()
        {
            _container.Register(
                typeof(IAzureSearch<>),
                typeof(AzureSearch<>));

            return this;
        }

        public AzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IAzureModel, new()
            where TSearcher : IAzureSearch<TDocument>
        {
            _container.Register(typeof(TSearcherType), typeof(TSearcher), new PerContainerLifetime());

            return this;
        }
    }
}