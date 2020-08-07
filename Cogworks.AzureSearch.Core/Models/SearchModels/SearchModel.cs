using Cogworks.AzureSearch.Models;
using Microsoft.Azure.Search;
using System.ComponentModel.DataAnnotations;

namespace Cogworks.AzureSearch.Core.Models.SearchModels
{
    public abstract class SearchModel : IAzureModelIdentity
    {
        [Key, IsFilterable, IsSearchable]
        public string Id { get; set; }
    }
}