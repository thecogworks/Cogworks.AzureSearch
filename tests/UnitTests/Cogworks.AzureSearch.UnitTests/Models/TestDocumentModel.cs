using Azure.Search.Documents.Indexes;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.UnitTests.Models
{
    public class TestDocumentModel : IAzureModel
    {
        [SimpleField(IsKey = true, IsFilterable = true)]
        [SearchableField()]
        public string Id { get; set; }

        [SimpleField(IsFilterable = true)]
        [SearchableField()]

        public string Name { get; set; }
    }
}