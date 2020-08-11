﻿using Autofac;
using Cogworks.AzureSearch.Indexes;
using Cogworks.AzureSearch.Initializers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.Searchers;

namespace Cogworks.AzureSearch.Autofac.Extensions
{
    public static class AutofacExtensions
    {
        public static void RegisterInitializers(this ContainerBuilder builder)
            => builder.RegisterGeneric(typeof(AzureInitializer<>))
                .As(typeof(IAzureInitializer<>))
                .InstancePerDependency();

        public static void RegisterIndexOptions(this ContainerBuilder builder, bool recreate, bool recreateOnUpdateFailure = false)
            => builder.Register(_ => new AzureSearchIndexOption(recreate, recreateOnUpdateFailure))
                .AsSelf()
                .SingleInstance();

        public static void RegisterClientOptions(this ContainerBuilder builder, string serviceName, string credentials)
            => builder.Register(_ => new AzureSearchClientOption(serviceName, credentials))
                .AsSelf()
                .SingleInstance();

        public static void RegisterIndexDefinitions<TDocument>(this ContainerBuilder builder, string indexName)
            where TDocument : class, IAzureModel, new()
            => builder.Register(_ => new AzureIndexDefinition<TDocument>(indexName))
                .AsSelf()
                .SingleInstance();

        public static void RegisterIndexes(this ContainerBuilder builder)
            => builder.RegisterGeneric(typeof(AzureIndex<>))
                .As(typeof(IAzureIndex<>))
                .InstancePerDependency();

        public static void RegisterRepositories(this ContainerBuilder builder)
            => builder.RegisterGeneric(typeof(AzureSearchRepository<>))
                .As(typeof(IAzureSearchRepository<>))
                .As(typeof(IAzureIndexOperation<>))
                .As(typeof(IAzureDocumentOperation<>))
                .As(typeof(IAzureDocumentSearch<>))
                .InstancePerDependency();

        public static void RegisterSearchers(this ContainerBuilder builder)
            => builder.RegisterGeneric(typeof(AzureSearch<>))
                .As(typeof(IAzureSearch<>))
                .InstancePerDependency();

        public static void RegisterDomainSearcher<TSearcher, TSearcherType, TDocument>(this ContainerBuilder builder)
            where TDocument : class, IAzureModel, new()
            where TSearcher : IAzureSearch<TDocument>
            => builder.RegisterType<TSearcher>()
                .As<TSearcherType>()
                .AsSelf()
                .SingleInstance();
    }
}