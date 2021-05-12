using System;

namespace Cogworks.AzureSearch.Exceptions.IndexExceptions
{
    public class IndexExistsException : DomainException
    {
        public override string Code => "index_exists_exception";

        public IndexExistsException(string message) : base(message)
        {
        }

        public IndexExistsException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}