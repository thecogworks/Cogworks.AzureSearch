using Autofac;
using Cogworks.AzureSearch.Core.Models.SearchModels;
using Cogworks.AzureSearch.Indexers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;
using Cogworks.AzureSearch.Repositories;

namespace Cogworks.AzureSearch.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            // Options Registration
            RegisterOptions(builder);

            // Index Registration
            RegisterIndexes(builder);

            // Repository registration
            RegisterRepositories(builder);

            // Indexer Registration
            RegisterIndexers(builder);

            // Searcher Registration

            // Initializers Register

            var container = builder.Build();

            var eventRepository = container.Resolve<IAzureSearchRepository<EventDocument>>();
            var newsRepository = container.Resolve<IAzureSearchRepository<NewsDocument>>();

            var azureEventIndexer = container.Resolve<IAzureIndexer<EventDocument>>();
            var azureNewsIndexer = container.Resolve<IAzureIndexer<NewsDocument>>();
        }

        private static void RegisterOptions(ContainerBuilder builder) =>
            builder
                .Register(_ => new AzureSearchClientOption("test", "testPassword"))
                .AsSelf()
                .SingleInstance();

        private static void RegisterIndexes(ContainerBuilder builder)
        {
            builder.Register(_ => new AzureIndex<EventDocument>("event")).AsSelf().SingleInstance();
            builder.Register(_ => new AzureIndex<NewsDocument>("news")).AsSelf().SingleInstance();
        }

        private static void RegisterIndexers(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(AzureIndexer<>))
                .As(typeof(IAzureIndexer<>))
                .InstancePerDependency();
        }

        private static void RegisterRepositories(ContainerBuilder builder)
        {
            builder.RegisterGeneric(typeof(AzureSearchRepository<>))
                .As(typeof(IAzureSearchRepository<>))
                .InstancePerDependency();
        }
    }
}