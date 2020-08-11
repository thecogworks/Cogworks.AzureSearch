using Autofac;
using Cogworks.AzureSearch.Autofac.Extensions;
using Cogworks.AzureSearch.Example.Core.Models.SearchModels;
using Cogworks.AzureSearch.Example.Core.Services.SearchServices;
using Cogworks.AzureSearch.Indexes;
using Cogworks.AzureSearch.Initializers;
using Cogworks.AzureSearch.Repositories;
using Cogworks.AzureSearch.Searchers;

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

            builder.RegisterSearchers();

            builder.RegisterInitializers();

            builder.RegisterIndexDefinitions<EventDocument>("event");
            builder.RegisterIndexDefinitions<NewsDocument>("news");

            builder.RegisterDomainSearcher<EventSearch, IEventSearch, EventDocument>();

            var container = builder.Build();

            var eventRepository = container.Resolve<IAzureSearchRepository<EventDocument>>();
            var newsRepository = container.Resolve<IAzureSearchRepository<NewsDocument>>();

            var newsDocumentOperation = container.Resolve<IAzureDocumentOperation<NewsDocument>>();

            var newsIndexOperation = container.Resolve<IAzureIndexOperation<NewsDocument>>();

            var azureEventIndex = container.Resolve<IAzureIndex<EventDocument>>();
            var azureNewsIndex = container.Resolve<IAzureIndex<NewsDocument>>();

            var initializer = container.Resolve<IAzureInitializer<EventDocument>>();

            var eventSearch = container.Resolve<IAzureSearch<EventDocument>>();

            var domainEventSearch = container.Resolve<IEventSearch>();

            domainEventSearch.SomeSearch();
        }
    }
}