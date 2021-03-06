﻿using Cogworks.AzureSearch.Interfaces.Indexes;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Indexes
{
    internal class AzureIndex<TAzureModel> : IAzureIndex<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        private readonly IAzureDocumentOperation<TAzureModel> _azureSearchRepository;

        public AzureIndex(IAzureDocumentOperation<TAzureModel> azureSearchRepository)
            => _azureSearchRepository = azureSearchRepository;

        public async Task<AzureDocumentOperationResult> AddOrUpdateDocumentAsync(TAzureModel model)
            => await _azureSearchRepository.AddOrUpdateDocumentAsync(model);

        public async Task<AzureDocumentOperationResult> TryRemoveDocumentAsync(TAzureModel model)
            => await _azureSearchRepository.TryRemoveDocumentAsync(model);

        public async Task<AzureBatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _azureSearchRepository.AddOrUpdateDocumentsAsync(models);

        public async Task<AzureBatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _azureSearchRepository.TryRemoveDocumentsAsync(models);
    }
}