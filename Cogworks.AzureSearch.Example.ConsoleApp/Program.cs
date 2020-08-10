using Autofac;
using Cogworks.AzureSearch.Autofac.Extensions;
using Cogworks.AzureSearch.Example.Core.Models.SearchModels;
using Cogworks.AzureSearch.Indexes;
using Cogworks.AzureSearch.Initializers;
using Cogworks.AzureSearch.Repositories;

namespace Cogworks.AzureSearch.Example.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = new ContainerBuilder();

            builder.RegisterIndexOptions(true, false);

            builder.RegisterClientOptions("test", "testPassword");

            builder.RegisterRepositories();

            builder.RegisterIndexes();

            builder.RegisterInitializers();

            builder.RegisterIndexDefinitions<EventDocument>("event");
            builder.RegisterIndexDefinitions<NewsDocument>("news");

            var container = builder.Build();

            var eventRepository = container.Resolve<IAzureSearchRepository<EventDocument>>();
            var newsRepository = container.Resolve<IAzureSearchRepository<NewsDocument>>();

            var azureEventIndex = container.Resolve<IAzureIndex<EventDocument>>();
            var azureNewsIndex = container.Resolve<IAzureIndex<NewsDocument>>();

            var initializer = container.Resolve<IAzureInitializer<EventDocument>>();
        }
    }
}