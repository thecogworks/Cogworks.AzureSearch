using Autofac;
using Cogworks.AzureSearch.Indexes;
using Cogworks.AzureSearch.Initializers;
using Cogworks.AzureSearch.Interfaces;
using Cogworks.AzureSearch.Interfaces.Wrappers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.Searchers;
using Cogworks.AzureSearch.Wrappers;

namespace Cogworks.AzureSearch.Autofac.Extensions
{
    public class AzureSearchBuilder
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

        public AzureSearchBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false)
        {
            _builder.Register(_ => new AzureSearchIndexOption(recreate, recreateOnUpdateFailure))
                .AsSelf()
                .SingleInstance();

            return this;
        }

        public AzureSearchBuilder RegisterClientOptions(string serviceName, string credentials)
        {
            _builder.Register(_ => new AzureSearchClientOption(serviceName, credentials))
                .AsSelf()
                .SingleInstance();

            return this;
        }

        public AzureSearchBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IAzureModel, new()
        {
            _builder.Register(_ => new AzureIndexDefinition<TDocument>(indexName))
                .AsSelf()
                .SingleInstance();

            return this;
        }

        internal AzureSearchBuilder RegisterIndexes()
        {
            _builder.RegisterGeneric(typeof(AzureIndex<>))
                .As(typeof(IAzureIndex<>))
                .InstancePerDependency();

            return this;
        }

        internal AzureSearchBuilder RegisterWrappers()
        {
            _builder.RegisterGeneric(typeof(DocumentOperationWrapper<>))
                .As(typeof(IDocumentOperationWrapper<>))
                .InstancePerDependency();

            _builder.RegisterType<IndexOperationWrapper>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            return this;
        }

        internal AzureSearchBuilder RegisterRepositories()
        {
            _builder.RegisterGeneric(typeof(AzureSearchRepository<>))
                .As(typeof(IAzureSearchRepository<>))
                .As(typeof(IAzureIndexOperation<>))
                .As(typeof(IAzureDocumentOperation<>))
                .As(typeof(IAzureDocumentSearch<>))
                .InstancePerDependency();

            return this;
        }

        internal AzureSearchBuilder RegisterSearchers()
        {
            _builder.RegisterGeneric(typeof(AzureSearch<>))
                .As(typeof(IAzureSearch<>))
                .InstancePerDependency();

            return this;
        }

        public AzureSearchBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IAzureModel, new()
            where TSearcher : IAzureSearch<TDocument>
        {
            _builder.RegisterType<TSearcher>()
                .As<TSearcherType>()
                .AsSelf()
                .SingleInstance();

            return this;
        }
    }

    public static class AutofacExtensions
    {
        public static AzureSearchBuilder RegisterAzureSearch(this ContainerBuilder builder)
            => new AzureSearchBuilder(builder)
                .RegisterRepositories()
                .RegisterIndexes()
                .RegisterSearchers()
                .RegisterInitializers()
                .RegisterWrappers();
    }
}