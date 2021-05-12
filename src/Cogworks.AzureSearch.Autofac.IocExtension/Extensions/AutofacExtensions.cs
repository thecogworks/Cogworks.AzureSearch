using Autofac;
using Cogworks.AzureSearch.Autofac.Builders;

namespace Cogworks.AzureSearch.Autofac.Extensions
{
    public static class AutofacExtensions
    {
        public static AzureSearchBuilder RegisterAzureSearch(this ContainerBuilder builder)
            => new AzureSearchBuilder(builder)
                .RegisterRepositories()
                .RegisterIndexes()
                .RegisterSearchers()
                .RegisterInitializers()
                .RegisterWrappers()
                .RegisterOperations();
    }
}