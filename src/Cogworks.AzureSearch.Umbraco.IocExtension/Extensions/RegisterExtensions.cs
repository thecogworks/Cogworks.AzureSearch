using Cogworks.AzureSearch.Umbraco.IocExtension.Builders;
using LightInject;
using Umbraco.Core.Composing;
using LightInjectAzureSearchBuilder = Cogworks.AzureSearch.LightInject.IocExtension.Builders.AzureSearchBuilder;

namespace Cogworks.AzureSearch.Umbraco.IocExtension.Extensions
{
    public static class RegisterExtensions
    {
        public static AzureSearchBuilder RegisterAzureSearch(this Composition composingRegister)
            => new AzureSearchBuilder(composingRegister)
                .RegisterRepositories()
                .RegisterIndexes()
                .RegisterSearchers()
                .RegisterInitializers()
                .RegisterWrappers();

        public static LightInjectAzureSearchBuilder RegisterAzureSearch(this IRegister composingRegister)
            => new LightInjectAzureSearchBuilder(composingRegister.Concrete as ServiceContainer)
                .RegisterRepositories()
                .RegisterIndexes()
                .RegisterSearchers()
                .RegisterInitializers()
                .RegisterWrappers();
    }
}