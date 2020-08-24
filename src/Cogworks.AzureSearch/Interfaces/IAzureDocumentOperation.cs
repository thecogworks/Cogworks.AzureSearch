using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Interfaces
{
    public interface IAzureDocumentOperation<in TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        Task<AzureDocumentOperationResult> AddOrUpdateDocumentAsync(TAzureModel model);

        Task<AzureBatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models);

        Task<AzureDocumentOperationResult> TryRemoveDocumentAsync(TAzureModel model);

        Task<AzureBatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models);
    }
}