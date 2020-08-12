using Cogworks.AzureSearch.Enums;
using System.Collections.Generic;

namespace Cogworks.AzureSearch.Models
{
    public class AzureSearchParameters
    {
        public bool IncludeTotalResultCount { get; set; }

        public IList<string> Facets { get; set; }

        public string Filter { get; set; }

        public IList<string> HighlightFields { get; set; }

        public string HighlightPostTag { get; set; }

        public string HighlightPreTag { get; set; }

        public double? MinimumCoverage { get; set; }

        public IList<string> OrderBy { get; set; }

        public AzureQueryType QueryType { get; set; }

        public string ScoringProfile { get; set; }

        public IList<string> SearchFields { get; set; }

        public AzureSearchModeType SearchMode { get; set; }

        public IList<string> Select { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}