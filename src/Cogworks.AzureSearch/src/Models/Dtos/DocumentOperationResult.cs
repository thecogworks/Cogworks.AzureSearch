namespace Cogworks.AzureSearch.Models.Dtos
{
    public class DocumentOperationResult
    {
        public bool Succeeded { get; set; }

        public string ModelId { get; set; }

        public string Message { get; set; }

        public string InnerMessage { get; set; }

        public int StatusCode { get; set; }
    }
}