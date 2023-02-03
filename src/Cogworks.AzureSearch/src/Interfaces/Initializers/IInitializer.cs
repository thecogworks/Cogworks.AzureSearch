using Cogworks.AzureSearch.Models;
using System.Threading.Tasks;

namespace Cogworks.AzureSearch.Interfaces.Initializers
{
    public interface IInitializer<in TModel>
        where TModel : class, IModel, new()
    {
        Task InitializeAsync();
    }
}