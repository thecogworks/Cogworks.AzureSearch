using Autofac;
using AutofacContainerBuilder = Autofac.ContainerBuilder;
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

namespace Cogworks.AzureSearch.Autofac.Builders
{

    public class ContainerBuilder : IContainerBuilder
    {
        private readonly AutofacContainerBuilder _builder;

        public ContainerBuilder(AutofacContainerBuilder builder)
            => _builder = builder;

        internal ContainerBuilder RegisterInitializers()
        {
            _builder.RegisterGeneric(typeof(Initializer<>))
                .As(typeof(IInitializer<>))
                .InstancePerDependency();

            return this;
        }

        public IContainerBuilder RegisterIndexOptions(bool recreate, bool recreateOnUpdateFailure = false)
        {
            _ = _builder.Register(_ => new IndexOption(recreate, recreateOnUpdateFailure))
                .AsSelf()
                .SingleInstance();

            return this;
        }

        public IContainerBuilder RegisterClientOptions(string serviceName, string credentials, string serviceEndpointUrl)
        {
            _ = _builder.Register(_ => new ClientOption(
                    serviceName,
                    credentials,
                    serviceEndpointUrl))
                .AsSelf()
                .SingleInstance();

            return this;
        }

        public IContainerBuilder RegisterIndexDefinitions<TDocument>(string indexName)
            where TDocument : class, IModel, new()
        {
            _ = _builder.Register(_ => new IndexDefinition<TDocument>(indexName))
                .AsSelf()
                .SingleInstance();

            return this;
        }

        public IContainerBuilder RegisterIndexDefinitions<TDocument>(SearchIndex customIndex)
            where TDocument : class, IModel, new()
        {
            _ = _builder.Register(_ => new IndexDefinition<TDocument>(customIndex))
                .AsSelf()
                .SingleInstance();

            return this;
        }

        internal ContainerBuilder RegisterIndexes()
        {
            _ = _builder.RegisterGeneric(typeof(Index<>))
                .As(typeof(IIndex<>))
                .InstancePerDependency();

            return this;
        }

        internal ContainerBuilder RegisterWrappers()
        {
            _ = _builder.RegisterGeneric(typeof(DocumentOperationWrapper<>))
                .As(typeof(IDocumentOperationWrapper<>))
                .InstancePerDependency();

            _ = _builder.RegisterType<IndexOperationWrapper>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            return this;
        }

        internal ContainerBuilder RegisterRepositories()
        {
            _ = _builder.RegisterGeneric(typeof(Repository<>))
                .As(typeof(IRepository<>))
                .InstancePerDependency();

            return this;
        }

        internal ContainerBuilder RegisterSearchers()
        {
            _ = _builder.RegisterGeneric(typeof(Searcher<>))
                .As(typeof(ISearcher<>))
                .InstancePerDependency();

            return this;
        }

        internal ContainerBuilder RegisterOperations()
        {
            _ = _builder.RegisterGeneric(typeof(DocumentOperation<>))
                .As(typeof(IDocumentOperation<>))
                .InstancePerDependency();

            _ = _builder.RegisterGeneric(typeof(IndexOperation<>))
                .As(typeof(IIndexOperation<>))
                .InstancePerDependency();

            return this;
        }

        public IContainerBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>()
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _ = _builder.RegisterType<TSearcher>()
                .As<TSearcherType>()
                .AsSelf()
                .SingleInstance();

            return this;
        }

        public IContainerBuilder RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(TSearcherType instance)
            where TDocument : class, IModel, new()
            where TSearcher : BaseDomainSearch<TDocument>, TSearcherType
            where TSearcherType : class
        {
            _ = _builder.RegisterInstance(instance).As<TSearcherType>();

            return this;
        }
    }
}