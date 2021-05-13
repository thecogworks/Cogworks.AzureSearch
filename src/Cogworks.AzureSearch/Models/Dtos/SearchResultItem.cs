using System.Collections.Generic;

namespace Cogworks.AzureSearch.Models.Dtos
{
    public class SearchResultItem<TModel> where TModel : class, IModel, new()
    {
        public TModel Document { get; }

        public IDictionary<string, IList<string>> Highlights { get; }

        public double Score { get; }

        public SearchResultItem(TModel document, IDictionary<string, IList<string>> highlights, double score)
        {
            Document = document;
            Highlights = highlights;
            Score = score;
        }
    }
}