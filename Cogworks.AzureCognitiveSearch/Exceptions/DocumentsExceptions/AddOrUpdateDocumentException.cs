using System;

namespace Cogworks.AzureCognitiveSearch.Exceptions.DocumentsExceptions
{
    public class AddOrUpdateDocumentException : DomainException
    {

        public override string Code => "add_or_update_document_exception";

        public AddOrUpdateDocumentException(string message) : base(message)
        {
        }

        public AddOrUpdateDocumentException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}