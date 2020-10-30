using System.ComponentModel.DataAnnotations;
using Cogworks.AzureSearch.Models;
using Microsoft.Azure.Search;

namespace Cogworks.AzureSearch.LightInject.UnitTests.Models
{
    public class ThirdTestDocumentModel : IAzureModel
    {
        [Key, IsFilterable, IsRetrievable(true), IsSearchable]
        public string Id { get; set; }

        [IsFilterable, IsSearchable]
        public string Name { get; set; }
    }
}