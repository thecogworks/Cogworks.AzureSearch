using Cogworks.AzureSearch.LightInject.IocExtension.Builders;
using LightInject;

namespace Cogworks.AzureSearch.LightInject.IocExtension.Extensions
{
    public static class RegisterExtensions
    {
        public static ContainerBuilder RegisterAzureSearch(this IServiceContainer serviceContainer)
            => new ContainerBuilder(serviceContainer)
                .RegisterRepositories()
                .RegisterIndexes()
                .RegisterSearchers()
                .RegisterInitializers()
                .RegisterWrappers()
                .RegisterOperations();
    }
}