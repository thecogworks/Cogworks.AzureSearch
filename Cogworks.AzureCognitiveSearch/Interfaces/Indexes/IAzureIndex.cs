
using System.Collections.Generic;
using System.Threading.Tasks;
using Cogworks.AzureCognitiveSearch.Models;
using Cogworks.AzureCognitiveSearch.Models.Dtos;

namespace Cogworks.AzureCognitiveSearch.Interfaces.Indexes
{
    public interface IAzureIndex<in TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        Task<AzureDocumentOperationResult> AddOrUpdateDocumentAsync(TAzureModel model);

        Task<AzureBatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models);

        Task<AzureDocumentOperationResult> TryRemoveDocumentAsync(TAzureModel model);

        Task<AzureBatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models);
    }
}