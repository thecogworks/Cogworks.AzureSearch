using Cogworks.AzureSearch.Models;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Interfaces
{
    public interface IAzureDocumentSearch<TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        Models.Dtos.SearchResult<TAzureModel> Search(string keyword, AzureSearchParameters azureSearchParameters);

        Task<Models.Dtos.SearchResult<TAzureModel>> SearchAsync(string keyword, AzureSearchParameters azureSearchParameters);
    }
}