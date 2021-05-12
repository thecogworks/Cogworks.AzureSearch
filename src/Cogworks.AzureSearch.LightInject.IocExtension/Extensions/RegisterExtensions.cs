using Cogworks.AzureSearch.LightInject.IocExtension.Builders;
using LightInject;

namespace Cogworks.AzureSearch.LightInject.IocExtension.Extensions
{
    public static class RegisterExtensions
    {
        public static AzureSearchBuilder RegisterAzureSearch(this IServiceContainer serviceContainer)
            => new AzureSearchBuilder(serviceContainer)
                .RegisterRepositories()
                .RegisterIndexes()
                .RegisterSearchers()
                .RegisterInitializers()
                .RegisterWrappers()
                .RegisterOperations();
    }
}