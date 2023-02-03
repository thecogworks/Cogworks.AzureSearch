using Cogworks.AzureSearch.Interfaces.Builder;
using Cogworks.AzureSearch.IoC.Umbraco.Builders;
using Umbraco.Cms.Core.DependencyInjection;

namespace Cogworks.AzureSearch.IoC.Umbraco.Extensions
{
    public static class RegisterExtensions
    {
        public static IContainerBuilder RegisterAzureSearch(this IUmbracoBuilder umbracoBuilder)
            => new ContainerBuilder(umbracoBuilder)
                    .RegisterRepositories()
                    .RegisterIndexes()
                    .RegisterSearchers()
                    .RegisterInitializers()
                    .RegisterWrappers()
                    .RegisterOperations();
    }
}