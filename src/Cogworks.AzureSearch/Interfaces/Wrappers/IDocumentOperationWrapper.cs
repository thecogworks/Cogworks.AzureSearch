﻿using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Interfaces.Wrappers
{
    public interface IDocumentOperationWrapper<TAzureModel> where TAzureModel : class, IModel, new()
    {
        SearchResults<TAzureModel> Search(string searchText, SearchOptions parameters = null);

        Task<SearchResults<TAzureModel>> SearchAsync(string searchText, SearchOptions parameters = null);

        Task<Response<IndexDocumentsResult>> IndexAsync(IndexDocumentsBatch<TAzureModel> indexBatch);
    }
}