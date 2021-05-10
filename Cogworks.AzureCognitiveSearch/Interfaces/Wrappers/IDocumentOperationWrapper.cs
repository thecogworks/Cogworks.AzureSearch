using System.Threading.Tasks;
using Azure;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Cogworks.AzureCognitiveSearch.Models;

namespace Cogworks.AzureCognitiveSearch.Interfaces.Wrappers
{
    public interface IDocumentOperationWrapper<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        SearchResults<TAzureModel> Search(string searchText, SearchOptions parameters = null);

        Task<Response<SearchResults<TAzureModel>>> SearchAsync(string searchText, SearchOptions parameters = null);

        Task<Response<IndexDocumentsResult>> IndexAsync(IndexDocumentsBatch<TAzureModel> indexBatch);
    }
}