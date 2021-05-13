using Cogworks.AzureSearch.Interfaces.Builder;
using Cogworks.AzureSearch.IoC.LightInject.Extensions;
using Cogworks.AzureSearch.IoC.Umbraco.Builders;
using LightInject;
using Umbraco.Core.Composing;

namespace Cogworks.AzureSearch.IoC.Umbraco.Extensions
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