using Microsoft.Azure.Search;

namespace Cogworks.AzureSearch.Options
{
    public class AzureSearchClientOption
    {
        public string ServiceName { get; }

        public string Credentials { get; }

        public AzureSearchClientOption(string serviceName, string credentials)
        {
            ServiceName = serviceName;
            Credentials = credentials;
        }

        public SearchServiceClient GetSearchServiceClient()
            => new SearchServiceClient(
                ServiceName,
                new SearchCredentials(Credentials));
    }
}