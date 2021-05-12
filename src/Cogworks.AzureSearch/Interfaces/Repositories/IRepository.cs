using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Interfaces.Repositories
{
    public interface IRepository<TAzureModel> :
        IDocumentOperation<TAzureModel>,
        IIndexOperation<TAzureModel>,
        ISearcher<TAzureModel>
        where TAzureModel : class, IModel, new()
    {
    }
}