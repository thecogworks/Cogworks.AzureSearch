namespace Cogworks.AzureSearch.Models.Dtos
{
    public class AzureRemoveResultDto
    {
        public bool Succeeded { get; set; }

        public string ModelId { get; set; }

        public string Message { get; set; }
    }
}