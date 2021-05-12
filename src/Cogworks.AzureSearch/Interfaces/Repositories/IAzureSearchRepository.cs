using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Interfaces.Repositories
{
    public interface IAzureSearchRepository<TAzureModel> :
        IAzureDocumentOperation<TAzureModel>,
        IAzureIndexOperation<TAzureModel>,
        IAzureSearch<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
    }
}