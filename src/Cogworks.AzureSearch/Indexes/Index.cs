﻿using Cogworks.AzureSearch.Interfaces.Indexes;
using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Indexes
{
    internal class Index<TAzureModel> : IIndex<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        private readonly IAzureDocumentOperation<TAzureModel> _documentOperation;

        public Index(IAzureDocumentOperation<TAzureModel> documentOperation)
            => _documentOperation = documentOperation;

        public async Task<AzureDocumentOperationResult> AddOrUpdateDocumentAsync(TAzureModel model)
            => await _documentOperation.AddOrUpdateDocumentAsync(model);

        public async Task<AzureDocumentOperationResult> TryRemoveDocumentAsync(TAzureModel model)
            => await _documentOperation.TryRemoveDocumentAsync(model);

        public async Task<AzureBatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _documentOperation.AddOrUpdateDocumentsAsync(models);

        public async Task<AzureBatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models)
            => await _documentOperation.TryRemoveDocumentsAsync(models);
    }
}