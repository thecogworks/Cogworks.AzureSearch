using System.Collections.Generic;
using System.Linq;

namespace Cogworks.AzureSearch.Models.Dtos
{
    public class BatchDocumentsOperationResult
    {
        public bool Succeeded { get; set; }

        public string Message { get; set; }

        public IEnumerable<DocumentOperationResult> SucceededDocuments { get; set; } = Enumerable.Empty<DocumentOperationResult>();

        public IEnumerable<DocumentOperationResult> FailedDocuments { get; set; } = Enumerable.Empty<DocumentOperationResult>();
    }
}