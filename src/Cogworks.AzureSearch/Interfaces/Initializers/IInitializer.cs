using Cogworks.AzureSearch.Models;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Interfaces.Initializers
{
    public interface IInitializer<in TAzureModel> where TAzureModel : class, IAzureModel, new()
    {
        Task InitializeAsync();
    }
}