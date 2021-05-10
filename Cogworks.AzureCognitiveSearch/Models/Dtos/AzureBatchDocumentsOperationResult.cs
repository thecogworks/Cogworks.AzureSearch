using System.Collections.Generic;
using System.Linq;

namespace Cogworks.AzureCognitiveSearch.Models.Dtos
{
    public class AzureBatchDocumentsOperationResult
    {
        public bool Succeeded { get; set; }

        public string Message { get; set; }

        public IEnumerable<AzureDocumentOperationResult> SucceededDocuments { get; set; } = Enumerable.Empty<AzureDocumentOperationResult>();

        public IEnumerable<AzureDocumentOperationResult> FailedDocuments { get; set; } = Enumerable.Empty<AzureDocumentOperationResult>();
    }
}