using Cogworks.AzureSearch.Models;
using Microsoft.Azure.Search;
using System.ComponentModel.DataAnnotations;

namespace Cogworks.AzureSearch.Example.Core.Models.SearchModels
{
    public class NewsDocument : IAzureModel
    {
        [Key, IsFilterable, IsSearchable]
        public string ContentId { get; set; }
    }
}