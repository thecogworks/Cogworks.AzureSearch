using System;

namespace Cogworks.AzureSearch.Exceptions.DocumentsExceptions
{
    public class RemoveDocumentException : DomainException
    {
        public override string Code => "remove_document_exception";

        public RemoveDocumentException(string message) : base(message)
        {
        }

        public RemoveDocumentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}