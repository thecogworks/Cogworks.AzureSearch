using Cogworks.AzureCognitiveSearch.Interfaces.Operations;
using Cogworks.AzureCognitiveSearch.Models;

namespace Cogworks.AzureCognitiveSearch.Interfaces.Repositories
{
    public interface IAzureSearchRepository<TAzureModel> :
        IAzureDocumentOperation<TAzureModel>,
        IAzureIndexOperation<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
    }
}