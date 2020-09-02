using Cogworks.AzureSearch.Builder;
using Cogworks.AzureSearch.Umbraco.IocExtension.Builders;
using LightInject;
using Umbraco.Core.Composing;
using LightInjectAzureSearchBuilder = Cogworks.AzureSearch.LightInject.IocExtension.Builders.AzureSearchBuilder;

namespace Cogworks.AzureSearch.Umbraco.IocExtension.Extensions
{
    public static class RegisterExtensions
    {
        public static IAzureSearchBuilder RegisterAzureSearch(this IRegister composingRegister, bool useConcreteContainer = false)
            => !useConcreteContainer
                ? new AzureSearchBuilder(composingRegister)
                    .RegisterRepositories()
                    .RegisterIndexes()
                    .RegisterSearchers()
                    .RegisterInitializers()
                    .RegisterWrappers()
                : new LightInjectAzureSearchBuilder(composingRegister.Concrete as ServiceContainer)
                    .RegisterRepositories()
                    .RegisterIndexes()
                    .RegisterSearchers()
                    .RegisterInitializers()
                    .RegisterWrappers() as IAzureSearchBuilder;
    }
}