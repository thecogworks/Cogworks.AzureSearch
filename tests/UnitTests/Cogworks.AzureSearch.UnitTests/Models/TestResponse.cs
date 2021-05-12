using Azure;

namespace Cogworks.AzureSearch.UnitTests.Models
{

    public class TestResponse<T> : Response<T>
    {

        public TestResponse(T value)
        {
            Value = value;
        }

        public override T Value { get; }

        public override Response GetRawResponse() => null;
    }
}