using Cogworks.AzureSearch.Models;
using Microsoft.Azure.Search;
using System.ComponentModel.DataAnnotations;

namespace Cogworks.AzureSearch.UmbracoIoc.UnitTests.Models
{
    public class NotRegisteredTestDocumentModel : IAzureModel
    {
        [Key, IsFilterable, IsRetrievable(true), IsSearchable]
        public string Id { get; set; }

        [IsFilterable, IsSearchable]
        public string Name { get; set; }
    }
}