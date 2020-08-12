using Cogworks.AzureSearch.Enums;

namespace Cogworks.AzureSearch.Models.Requests
{
    public abstract class BaseRequestModel
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public OrderType OrderType { get; set; }
    }
}