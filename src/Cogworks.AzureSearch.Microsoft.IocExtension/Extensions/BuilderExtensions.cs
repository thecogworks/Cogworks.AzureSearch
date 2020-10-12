using Cogworks.AzureSearch.Microsoft.IocExtension.Builders;
using Microsoft.Extensions.DependencyInjection;

namespace Cogworks.AzureSearch.Microsoft.IocExtension.Extensions
{
    public static class BuilderExtensions
    {
        public static AzureSearchBuilder RegisterAzureSearch(this IServiceCollection serviceCollection)
            => new AzureSearchBuilder(serviceCollection)
                .RegisterRepositories()
                .RegisterIndexes()
                .RegisterSearchers()
                .RegisterInitializers()
                .RegisterWrappers();
    }
}