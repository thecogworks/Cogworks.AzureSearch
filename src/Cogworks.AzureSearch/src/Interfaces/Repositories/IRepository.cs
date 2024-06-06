using Cogworks.AzureSearch.Interfaces.Operations;
using Cogworks.AzureSearch.Interfaces.Searches;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Interfaces.Repositories
{
    public interface IRepository<TModel> :
        IDocumentOperation<TModel>,
        IIndexOperation<TModel>,
        ISearcher<TModel>
        where TModel : class, IModel, new()
    {
    }
}