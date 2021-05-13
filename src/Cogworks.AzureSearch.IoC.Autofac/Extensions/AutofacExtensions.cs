using Cogworks.AzureSearch.IoC.Autofac.Builders;
using AutofacContainerBuilder = Autofac.ContainerBuilder;

namespace Cogworks.AzureSearch.IoC.Autofac.Extensions
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