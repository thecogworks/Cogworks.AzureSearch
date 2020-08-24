using Cogworks.AzureSearch.Models;
using Microsoft.Azure.Search.Models;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Interfaces.Wrappers
{
    public interface IDocumentOperationWrapper<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        DocumentSearchResult<TAzureModel> Search(string searchText, SearchParameters parameters = null);

        Task<DocumentSearchResult<TAzureModel>> SearchAsync(string searchText, SearchParameters parameters = null);

        Task<DocumentIndexResult> IndexAsync(IndexBatch<TAzureModel> indexBatch);
    }
}