using System.Threading.Tasks;
using Cogworks.AzureCognitiveSearch.Models;

namespace Cogworks.AzureCognitiveSearch.Interfaces.Searches
{
    public interface IAzureDocumentSearch<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        Models.Dtos.SearchResult<TAzureModel> Search(string keyword, AzureSearchParameters azureSearchParameters);

        Task<Models.Dtos.SearchResult<TAzureModel>> SearchAsync(string keyword, AzureSearchParameters azureSearchParameters);
    }
}