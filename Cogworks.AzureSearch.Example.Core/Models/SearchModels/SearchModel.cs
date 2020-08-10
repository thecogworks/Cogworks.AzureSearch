using System.ComponentModel.DataAnnotations;
using Cogworks.AzureSearch.Models;
using Microsoft.Azure.Search;

namespace Cogworks.AzureSearch.Example.Core.Models.SearchModels
{
    public abstract class SearchModel : IAzureModelIdentity
    {
        [Key, IsFilterable, IsSearchable]
        public string Id { get; set; }
    }
}