using Cogworks.AzureSearch.Microsoft.IocExtension.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Cogworks.AzureSearch.Microsoft.IocExtension.Extensions
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