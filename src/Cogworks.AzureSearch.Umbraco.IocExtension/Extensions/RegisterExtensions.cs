using Cogworks.AzureSearch.Umbraco.IocExtension.Builders;
using Umbraco.Core.Composing;

namespace Cogworks.AzureSearch.Umbraco.IocExtension.Extensions
{
    public static class RegisterExtensions
    {
        public static AzureSearchBuilder RegisterAzureSearch(this IRegister composingRegister)
            => new AzureSearchBuilder(composingRegister)
                .RegisterRepositories()
                .RegisterIndexes()
                .RegisterSearchers()
                .RegisterInitializers()
                .RegisterWrappers();
    }
}