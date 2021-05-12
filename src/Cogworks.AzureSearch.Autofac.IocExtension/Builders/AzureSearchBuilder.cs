using Autofac;
using Azure.Search.Documents.Indexes.Models;
using Cogworks.AzureSearch.Builder;
using Cogworks.AzureSearch.Indexes;
using Cogworks.AzureSearch.Initializers;
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

namespace Cogworks.AzureSearch.Autofac.Builders
{
    public class AzureSearchBuilder : IAzureSearchBuilder
    {
        private readonly ContainerBuilder _builder;

        public AzureSearchBuilder(ContainerBuilder builder)
            => _builder = builder;

        internal AzureSearchBuilder RegisterInitializers()
        {
            _builder.RegisterGeneric(typeof(AzureInitializer<>))
                .As(typeof(IAzureInitializer<>))
                .InstancePerDependency();

            return this;
        }

        public IAzureSearchBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false)
        {
            _ = _builder.Register(_ => new AzureSearchIndexOption(recreate, recreateOnUpdateFailure))
                .AsSelf()
                .SingleInstance();

            return this;
        }

        public IAzureSearchBuilder RegisterClientOptions(string serviceName, string credentials, string serviceEndpointUrl)
        {
            _ = _builder.Register(_ => new AzureSearchClientOption(
                    serviceName,
                    credentials,
                    serviceEndpointUrl))
                .AsSelf()
                .SingleInstance();

            return this;
        }

        public IAzureSearchBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IAzureModel, new()
        {
            _ = _builder.Register(_ => new AzureIndexDefinition<TDocument>(indexName))
                .AsSelf()
                .SingleInstance();

            return this;
        }

        public IAzureSearchBuilder RegisterIndexDefinitions<TDocument>(SearchIndex customIndex)
            where TDocument : class, IAzureModel, new()
        {
            _ = _builder.Register(_ => new AzureIndexDefinition<TDocument>(customIndex))
                .AsSelf()
                .SingleInstance();

            return this;
        }

        internal AzureSearchBuilder RegisterIndexes()
        {
            _ = _builder.RegisterGeneric(typeof(Index<>))
                .As(typeof(IIndex<>))
                .InstancePerDependency();

            return this;
        }

        internal AzureSearchBuilder RegisterWrappers()
        {
            _ = _builder.RegisterGeneric(typeof(DocumentOperationWrapper<>))
                .As(typeof(IDocumentOperationWrapper<>))
                .InstancePerDependency();

            _ = _builder.RegisterType<IndexOperationWrapper>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            return this;
        }

        internal AzureSearchBuilder RegisterRepositories()
        {
            _ = _builder.RegisterGeneric(typeof(AzureSearchRepository<>))
                .As(typeof(IAzureSearchRepository<>))
                .InstancePerDependency();

            return this;
        }

        internal AzureSearchBuilder RegisterSearchers()
        {
            _ = _builder.RegisterGeneric(typeof(AzureSearch<>))
                .As(typeof(IAzureSearch<>))
                .InstancePerDependency();

            return this;
        }

        internal AzureSearchBuilder RegisterOperations()
        {
            _ = _builder.RegisterGeneric(typeof(AzureDocumentOperation<>))
                .As(typeof(IAzureDocumentOperation<>))
                .InstancePerDependency();

            _ = _builder.RegisterGeneric(typeof(AzureIndexOperation<>))
                .As(typeof(IAzureIndexOperation<>))
                .InstancePerDependency();

            return this;
        }

        public IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IAzureModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _ = _builder.RegisterType<TSearcher>()
                .As<TSearcherType>()
                .AsSelf()
                .SingleInstance();

            return this;
        }

        public IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(TSearcherType instance)
            where TDocument : class, IAzureModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _ = _builder.RegisterInstance(instance).As<TSearcherType>();

            return this;
        }
    }
}