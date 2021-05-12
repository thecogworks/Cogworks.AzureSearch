using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Cogworks.AzureSearch.Models;

namespace Cogworks.AzureSearch.Interfaces.Wrappers
{
    public interface IDocumentOperationWrapper<TModel> where TModel : class, IModel, new()
    {
        SearchResults<TModel> Search(string searchText, SearchOptions parameters = null);

        Task<SearchResults<TModel>> SearchAsync(string searchText, SearchOptions parameters = null);

        Task<Response<IndexDocumentsResult>> IndexAsync(IndexDocumentsBatch<TModel> indexBatch);
    }
}