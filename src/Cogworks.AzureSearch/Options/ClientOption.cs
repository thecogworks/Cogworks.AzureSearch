using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Search.Documents;

namespace Cogworks.AzureSearch.Options
{
    public class ClientOption
    {
        public string ServiceName { get; }

        public string Credentials { get; }

        public string ServiceUrlEndpoint { get; }

        public bool UseTokenCredentials { get; }

        public SearchClientOptions ClientOptions { get; }

        public ClientOption(string serviceName, string credentials, string serviceUrlEndpoint, bool searchHeaders = false)
        {
            ServiceName = serviceName;
            Credentials = credentials;
            ServiceUrlEndpoint = serviceUrlEndpoint;
            ClientOptions = GetOptions(searchHeaders);
        }

        private static SearchClientOptions GetOptions(bool searchHeaders)
        {
            var clientOptions = new SearchClientOptions();

            if (searchHeaders)
            {
                clientOptions.AddPolicy(
                    new SearchIdPipelinePolicy(),
                    HttpPipelinePosition.PerCall);
            }

            return clientOptions;
        }

        private class SearchIdPipelinePolicy : HttpPipelineSynchronousPolicy
        {
            public override void OnSendingRequest(HttpMessage message)
                => message.Request
                    .Headers
                    .SetValue("x-ms-azs-return-searchid", "true");
        }
    }
}