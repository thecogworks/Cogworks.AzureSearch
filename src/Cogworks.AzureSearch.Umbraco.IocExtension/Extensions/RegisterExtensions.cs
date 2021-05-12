using Cogworks.AzureSearch.Builder;
using Cogworks.AzureSearch.LightInject.IocExtension.Extensions;
using Cogworks.AzureSearch.Umbraco.IocExtension.Builders;
using LightInject;
using Umbraco.Core.Composing;

namespace Cogworks.AzureSearch.Umbraco.IocExtension.Extensions
{
    public static class RegisterExtensions
    {
        public static IAzureSearchBuilder RegisterAzureSearch(this IRegister composingRegister, bool useConcreteContainer = false)
            => useConcreteContainer
                ? (composingRegister.Concrete as IServiceContainer).RegisterAzureSearch() as IAzureSearchBuilder
                : new AzureSearchBuilder(composingRegister)
                    .RegisterRepositories()
                    .RegisterIndexes()
                    .RegisterSearchers()
                    .RegisterInitializers()
                    .RegisterWrappers()
                    .RegisterOperations();
    }
}