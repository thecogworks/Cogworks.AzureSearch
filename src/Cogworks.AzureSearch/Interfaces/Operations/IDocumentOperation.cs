using System.Collections.Generic;
using System.Threading.Tasks;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Interfaces.Operations
{
    public interface IDocumentOperation<in TAzureModel> where TAzureModel : class, IModel, new()
    {
        Task<DocumentOperationResult> AddOrUpdateDocumentAsync(TAzureModel model);

        Task<BatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TAzureModel> models);

        Task<DocumentOperationResult> TryRemoveDocumentAsync(TAzureModel model);

        Task<BatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TAzureModel> models);
    }
}