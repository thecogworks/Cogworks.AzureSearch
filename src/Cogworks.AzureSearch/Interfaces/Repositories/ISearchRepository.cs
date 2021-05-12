using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Interfaces.Repositories
{
    public interface ISearchRepository<TAzureModel> :
        IDocumentOperation<TAzureModel>,
        IIndexOperation<TAzureModel>,
        IAzureSearch<TAzureModel>
        where TAzureModel : class, IAzureModel, new()
    {
    }
}