using System;

namespace Cogworks.AzureCognitiveSearch.Exceptions.IndexExceptions
{
    public class IndexDeleteException : DomainException
    {
        public override string Code => "index_delete_error";

        public IndexDeleteException(string message) : base(message)
        {
        }

        public IndexDeleteException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}