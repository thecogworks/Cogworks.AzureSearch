using Cogworks.AzureSearch.IoC.Microsoft.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Cogworks.AzureSearch.IoC.Microsoft.Extensions
{
    public static class BuilderExtensions
    {
        public static ContainerBuilder RegisterAzureSearch(this IServiceCollection serviceCollection)
            => new ContainerBuilder(serviceCollection)
                .RegisterRepositories()
                .RegisterIndexes()
                .RegisterSearchers()
                .RegisterInitializers()
                .RegisterWrappers()
                .RegisterOperations();
    }
}