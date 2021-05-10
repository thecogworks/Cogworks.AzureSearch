using System;

namespace Cogworks.AzureCognitiveSearch.Exceptions.IndexExceptions
{
    public class IndexInitializerException : DomainException
    {
        public override string Code => "index_initializer_exception";

        public IndexInitializerException(string message) : base(message)
        {
        }

        public IndexInitializerException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}