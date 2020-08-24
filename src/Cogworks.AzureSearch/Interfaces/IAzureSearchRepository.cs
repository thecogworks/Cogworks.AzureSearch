using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Interfaces
{
    public interface IAzureSearchRepository<TAzureModel> :
        IAzureDocumentOperation<TAzureModel>,
        IAzureIndexOperation<TAzureModel>,
        IAzureDocumentSearch<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
    }
}