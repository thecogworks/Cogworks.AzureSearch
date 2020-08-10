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

            // Index Definition Registration
            RegisterIndexDefinitions(builder);

            // Repository registration
            RegisterRepositories(builder);

            // Index Registration
            RegisterIndexes(builder);

            // Searcher Registration

            // Initializers Register

            var container = builder.Build();

            var eventRepository = container.Resolve<IAzureSearchRepository<EventDocument>>();
            var newsRepository = container.Resolve<IAzureSearchRepository<NewsDocument>>();

            var azureEventIndexer = container.Resolve<IAzureIndex<EventDocument>>();
            var azureNewsIndexer = container.Resolve<IAzureIndex<NewsDocument>>();
        }

        private static void RegisterOptions(ContainerBuilder builder)
            => builder
                .Register(_ => new AzureSearchClientOption("test", "testPassword"))
                .AsSelf()
                .SingleInstance();

        private static void RegisterIndexDefinitions(ContainerBuilder builder)
        {
            builder.Register(_ => new AzureIndexDefinition<EventDocument>("event")).AsSelf().SingleInstance();
            builder.Register(_ => new AzureIndexDefinition<NewsDocument>("news")).AsSelf().SingleInstance();
        }

        private static void RegisterIndexes(ContainerBuilder builder)
            => builder.RegisterGeneric(typeof(AzureIndex<>))
                .As(typeof(IAzureIndex<>))
                .InstancePerDependency();

        private static void RegisterRepositories(ContainerBuilder builder)
            => builder.RegisterGeneric(typeof(AzureSearchRepository<>))
                .As(typeof(IAzureSearchRepository<>))
                .InstancePerDependency();
    }
}