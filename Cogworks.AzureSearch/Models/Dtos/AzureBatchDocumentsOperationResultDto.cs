using System.Collections.Generic;
using System.Linq;

namespace Cogworks.AzureSearch.Models.Dtos
{
    public class AzureBatchDocumentsOperationResultDto
    {
        public bool Succeeded { get; set; }

        public string Message { get; set; }

        public IEnumerable<AzureDocumentOperationResultDto> SucceededDocuments { get; set; } = Enumerable.Empty<AzureDocumentOperationResultDto>();

        public IEnumerable<AzureDocumentOperationResultDto> FailedDocuments { get; set; } = Enumerable.Empty<AzureDocumentOperationResultDto>();
    }
}