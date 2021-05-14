using System.Collections.Generic;
using System.Threading.Tasks;
using Cogworks.AzureSearch.Models;
using Cogworks.AzureSearch.Models.Dtos;

namespace Cogworks.AzureSearch.Interfaces.Operations
{
    public interface IDocumentOperation<in TModel>
        where TModel : class, IModel, new()
    {
        Task<DocumentOperationResult> AddOrUpdateDocumentAsync(TModel model);

        Task<BatchDocumentsOperationResult> AddOrUpdateDocumentsAsync(IEnumerable<TModel> models);

        Task<DocumentOperationResult> TryRemoveDocumentAsync(TModel model);

        Task<BatchDocumentsOperationResult> TryRemoveDocumentsAsync(IEnumerable<TModel> models);
    }
}