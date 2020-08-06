using Cogworks.AzureSearch.Models;
using Microsoft.Azure.Search;
using System.ComponentModel.DataAnnotations;

namespace Cogworks.AzureSearch.Core.Models.SearchModels
{
    public class NewsDocument : IAzureModelIdentity
    {
        [Key, IsFilterable]
        public string Id { get; set; }
    }
}