using Cogworks.AzureCognitiveSearch.Interfaces.Operations;
using Cogworks.AzureCognitiveSearch.Interfaces.Searches;
using Cogworks.AzureCognitiveSearch.Models;

namespace Cogworks.AzureCognitiveSearch.Interfaces.Repositories
{
    public interface IAzureSearchRepository<TAzureModel> :
        IAzureDocumentOperation<TAzureModel>,
        IAzureIndexOperation<TAzureModel>,
        IAzureSearch<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
    }
}