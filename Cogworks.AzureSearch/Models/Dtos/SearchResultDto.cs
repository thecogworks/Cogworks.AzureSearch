﻿using System.Collections.Generic;

namespace Cogworks.AzureSearch.Models.Dtos
{
    public class SearchResultDto<TModel> where TModel : IAzureModelIdentity
    {
        public long? TotalCount { get; set; }

        public IEnumerable<TModel> Results { get; set; }

        public bool HasMoreItems { get; set; }
    }
}