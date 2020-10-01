using Autofac;
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

        public IAzureSearchBuilder RegisterClientOptions(string serviceName, string credentials)
        {
            _ = _builder.Register(_ => new AzureSearchClientOption(serviceName, credentials))
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

        internal AzureSearchBuilder RegisterIndexes()
        {
            _ = _builder.RegisterGeneric(typeof(AzureIndex<>))
                .As(typeof(IAzureIndex<>))
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
                .As(typeof(IAzureIndexOperation<>))
                .As(typeof(IAzureDocumentOperation<>))
                .As(typeof(IAzureDocumentSearch<>))
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

        public IAzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IAzureModel, new()
            where TSearcher : class, IAzureSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _ = _builder.RegisterType<TSearcher>()
                .As<TSearcherType>()
                .AsSelf()
                .SingleInstance();

            return this;
        }

        IAzureSearchBuilder IAzureSearchBuilder.RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(
            TSearcherType instance)
        {
            _ = _builder.RegisterInstance(instance).As<TSearcherType>();

            return this;
        }
    }
}