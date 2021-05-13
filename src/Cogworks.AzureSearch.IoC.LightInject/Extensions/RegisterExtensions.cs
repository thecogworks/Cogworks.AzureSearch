using Cogworks.AzureSearch.IoC.LightInject.Builders;
using LightInject;

namespace Cogworks.AzureSearch.IoC.LightInject.Extensions
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