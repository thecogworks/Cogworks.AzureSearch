﻿using Cogworks.AzureSearch.Models;
using Microsoft.Azure.Search.Models;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Interfaces.Wrappers
{
    public interface IIndexOperationWrapper
    {
        Task<bool> ExistsAsync(string indexName);

        Task DeleteAsync(string indexName);

        Task<Index> CreateOrUpdateAsync<TAzureModel>(string indexName) where TAzureModel : class, IAzureModel, new();
    }
}