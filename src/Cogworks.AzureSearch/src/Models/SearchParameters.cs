using System.Collections.Generic;
using Cogworks.AzureSearch.Enums;

namespace Cogworks.AzureSearch.Models
{
    public class SearchParameters
    {
        public bool IncludeTotalResultCount { get; set; }

        public IEnumerable<string> Facets { get; set; }

        public string Filter { get; set; }

        public IEnumerable<string> HighlightFields { get; set; }

        public string HighlightPostTag { get; set; }

        public string HighlightPreTag { get; set; }

        public double? MinimumCoverage { get; set; }

        public IEnumerable<string> OrderBy { get; set; }

        public QueryType QueryType { get; set; }

        public string ScoringProfile { get; set; }

        public IEnumerable<string> SearchFields { get; set; }

        public SearchModeType SearchMode { get; set; }

        public IEnumerable<string> Select { get; set; }

        public int Skip { get; set; }

        public int Take { get; set; }
    }
}