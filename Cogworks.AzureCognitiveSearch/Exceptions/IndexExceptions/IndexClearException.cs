using System;

namespace Cogworks.AzureCognitiveSearch.Exceptions.IndexExceptions
{
    public class IndexClearException : DomainException
    {
        public override string Code => "index_clear_error";

        public IndexClearException(string message) : base(message)
        {
        }

        public IndexClearException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}