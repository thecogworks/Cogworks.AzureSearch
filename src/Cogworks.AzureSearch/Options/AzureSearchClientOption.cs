using Azure.Core;
using Azure.Core.Pipeline;
using Azure.Search.Documents;

namespace Cogworks.AzureSearch.Options
{
    public class AzureSearchClientOption
    {
        public string ServiceName { get; }

        public string Credentials { get; }

        public string ServiceUrlEndpoint { get; }

        public SearchClientOptions ClientOptions { get; }

        public AzureSearchClientOption(string serviceName, string credentials, string serviceUrlEndpoint)
        {
            ServiceName = serviceName;
            Credentials = credentials;
            ServiceUrlEndpoint = serviceUrlEndpoint;
            ClientOptions = GetOptions();
        }

        private static SearchClientOptions GetOptions()
        {
            var clientOptions = new SearchClientOptions();

            clientOptions.AddPolicy(
                new SearchIdPipelinePolicy(),
                HttpPipelinePosition.PerCall);

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