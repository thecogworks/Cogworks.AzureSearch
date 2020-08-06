using Autofac;
using Cogworks.AzureSearch.Core.Models.SearchModels;
using Cogworks.AzureSearch.Indexers;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Options;
using Cogworks.AzureSearch.Wrappers;
using System;

namespace Cogworks.AzureSearch.ConsoleApp
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var builder = new Autofac.ContainerBuilder();

            // Options registration
            builder
                .Register(_ => new AzureSearchClientOption("test", "testPassword"))
                .AsSelf()
                .SingleInstance();

            // Wrapper / Repository registration

            builder.Register<IAzureSearchWrapper>(_ => new AzureSearchWrapper("test", "testPassword")).AsImplementedInterfaces();

            // Azure Index registration

            builder.Register(_ => new AzureIndex<EventDocument>("event")).AsSelf().SingleInstance();
            builder.Register(_ => new AzureIndex<NewsDocument>("news")).AsSelf().SingleInstance();

            // Registration dedicated Indexer

            builder
                .RegisterType<AzureIndexer<EventDocument>>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder
                .RegisterType<AzureIndexer<NewsDocument>>()
                .AsImplementedInterfaces()
                .SingleInstance();

            // Registration dedicated Searcher

            var container = builder.Build();

            var azureEventIndexer = container.Resolve<IAzureIndexer<EventDocument>>();
            var azureNewsIndexer = container.Resolve<IAzureIndexer<NewsDocument>>();
        }
    }
}