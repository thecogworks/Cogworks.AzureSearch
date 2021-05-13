using AutofacContainerBuilder = Autofac.ContainerBuilder;
using Cogworks.AzureSearch.Autofac.Builders;

namespace Cogworks.AzureSearch.Autofac.Extensions
{
    public static class AutofacExtensions
    {
        public static ContainerBuilder RegisterAzureSearch(this AutofacContainerBuilder builder)
            => new ContainerBuilder(builder)
                .RegisterRepositories()
                .RegisterIndexes()
                .RegisterSearchers()
                .RegisterInitializers()
                .RegisterWrappers()
                .RegisterOperations();
    }
}