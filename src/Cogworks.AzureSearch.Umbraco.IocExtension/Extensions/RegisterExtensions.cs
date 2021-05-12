using Cogworks.AzureSearch.Interfaces.Builder;
using Cogworks.AzureSearch.LightInject.IocExtension.Extensions;
using Cogworks.AzureSearch.Umbraco.IocExtension.Builders;
using LightInject;
using Umbraco.Core.Composing;

namespace Cogworks.AzureSearch.Umbraco.IocExtension.Extensions
{
    public static class RegisterExtensions
    {
        public static IContainerBuilder RegisterAzureSearch(this IRegister composingRegister, bool useConcreteContainer = false)
            => useConcreteContainer
                ? (composingRegister.Concrete as IServiceContainer).RegisterAzureSearch() as IContainerBuilder
                : new ContainerBuilder(composingRegister)
                    .RegisterRepositories()
                    .RegisterIndexes()
                    .RegisterSearchers()
                    .RegisterInitializers()
                    .RegisterWrappers()
                    .RegisterOperations();
    }
}