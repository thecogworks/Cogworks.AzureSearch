using System.Collections.Generic;
using Azure;
using Azure.Search.Documents.Models;

namespace Cogworks.AzureCognitiveSearch.UnitTests.Models
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